using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Qweree.Authentication.Sdk.Http;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Cdn.Sdk.Storage;
using Qweree.Gateway.WebApi.Infrastructure;
using Qweree.Gateway.WebApi.Infrastructure.Session;
using Qweree.Utils;
using Yarp.ReverseProxy.Transforms;

namespace Qweree.Gateway.WebApi;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<QwereeConfigurationDo>(Configuration.GetSection("Qweree"));

        services.AddHealthChecks();
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
        services.AddCors(options =>
        {
            options.AddPolicy("liberal", builder =>
            {
                builder.WithOrigins(Configuration["Origin"]!)
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        var proxyBuilder = services.AddReverseProxy();
        proxyBuilder.LoadFromConfig(Configuration.GetSection("ReverseProxy"))
            .AddTransforms(builderContext =>
            {
                var keepHost = bool.Parse(Configuration["ReverseProxy:OriginalHost"]!);
                builderContext.AddOriginalHost(keepHost);
            });

        services.AddSingleton<ISessionStorage, QwereeSessionStorage>();
        // services.AddSingleton<ISessionStorage, FileSystemSessionStorage>(_ => new FileSystemSessionStorage(Configuration["Qweree:SessionStorage"]));
        services.AddSingleton<HttpMessageHandler, HttpClientHandler>();
        services.AddSingleton(p =>
        {
            var httpHandler = p.GetRequiredService<HttpMessageHandler>();
            var oauth2Client = p.GetRequiredService<OAuth2Client>();
            var qwereeConfig = p.GetRequiredService<IOptions<QwereeConfigurationDo>>();
            var clientCredentials = new ClientCredentials
            {
                ClientId = qwereeConfig.Value.ClientId,
                ClientSecret = qwereeConfig.Value.ClientSecret
            };

            return new ClientCredentialsHandler(httpHandler, oauth2Client, clientCredentials, new MemoryTokenStorage());
        });
        services.AddSingleton(p =>
        {
            var handler = p.GetRequiredService<HttpMessageHandler>();
            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(new Uri(Configuration["Qweree:AuthUri"]!), "api/oauth2/")
            };
            return new OAuth2Client(client);
        });
        services.AddSingleton(p =>
        {

            var httpHandler = p.GetRequiredService<ClientCredentialsHandler>();
            var httpClient = new HttpClient(httpHandler)
            {
                BaseAddress = new Uri(Configuration["Qweree:CdnUri"]!)
            };
            return new StorageClient(httpClient);
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
    {
        ServicePointManager.ServerCertificateValidationCallback += (_, _, _, _) => true;
        app.UseHttpsRedirection();
        if (environment.IsDevelopment())
            app.UseCors("liberal");

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapReverseProxy(pipeline =>
            {
                pipeline.UseSessionAffinity();
                pipeline.Use(async (context, next) =>
                {
                    var storage = context.RequestServices.GetRequiredService<ISessionStorage>();
                    var cookie = context.Request.Cookies["Session"];

                    if (cookie is not null)
                    {
                        try
                        {
                            await using var stream = await storage.ReadAsync(cookie);
                            var tokenInfo = await JsonUtils.DeserializeAsync<TokenInfo>(stream);
                            context.Request.Headers.Authorization = $"Bearer {tokenInfo?.AccessToken}";
                        }
                        catch (Exception)
                        {
                            context.Response.StatusCode = 401;
                        }
                    }

                    await next();
                });
            });
        });
    }

}