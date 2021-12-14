using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Authentication.Sdk.Tokens;
using Qweree.Gateway.WebApi.Infrastructure.Session;
using Qweree.Utils;

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
                builder.WithOrigins("http://localhost:5000")
                    .AllowCredentials()
                    .WithHeaders("Content-Type");
            });
        });
        var proxyBuilder = services.AddReverseProxy();
        proxyBuilder.LoadFromConfig(Configuration.GetSection("ReverseProxy"));

        services.AddSingleton(_ => new SessionStorage("~/Work/chrobo/temp/session"));
        services.AddSingleton<HttpMessageHandler, HttpClientHandler>();
        services.AddScoped(_ =>
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(new Uri("http://localhost/auth/"), "api/oauth2/auth/")
            };
            return new OAuth2Client(client);
        });
    }
    public void Configure(IApplicationBuilder app)
    {
        app.UseCors("liberal");
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapReverseProxy(pipeline =>
            {
                pipeline.UseSessionAffinity();
                pipeline.Use((context, next) =>
                {
                    var storage = context.RequestServices.GetRequiredService<SessionStorage>();

                    var cookie = context.Request.Cookies["Session"];

                    if (cookie != null)
                    {
                        try
                        {
                            using var stream = storage.ReadAsync(cookie);
                            using var reader = new StreamReader(stream);
                            var tokenInfo = JsonUtils.Deserialize<TokenInfoDto>(reader.ReadToEnd());
                            context.Request.Headers.Authorization = $"Bearer {tokenInfo?.AccessToken}";
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }

                    return next();
                });
            });
        });
    }

}