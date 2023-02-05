using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Qweree.AspNet.Web.Swagger;
using Qweree.Authentication.Sdk.Http;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Authentication.Sdk.Session;
using Qweree.Cdn.Sdk.Storage;
using Qweree.Mongo;
using Qweree.PiccStash.WebApi.Domain;
using Qweree.PiccStash.WebApi.Infrastructure;
using Qweree.PiccStash.WebApi.Infrastructure.System;
using Qweree.Utils;
using ClientCredentials = Qweree.Authentication.Sdk.OAuth2.ClientCredentials;

namespace Qweree.PiccStash.WebApi;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<QwereeConfigurationDo>(Configuration.GetSection("Qweree"));

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddHealthChecks()
            .AddMongoHealthCheck(Configuration["Qweree:HealthCheckConnectionString"]!, "Database");

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
        services.AddSwaggerGen(options =>
        {
            options.OperationFilter<FileFromBodyOperationFilter>();
            options.SwaggerDoc("v1", new OpenApiInfo {Title = "Qweree.PiccStash.WebApi", Version = "v1"});

            options.AddSecurityDefinition("openid", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OpenIdConnect,
                OpenIdConnectUrl = new Uri(Configuration["Qweree:SwaggerOpenId"]!, UriKind.Absolute),
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "openid"
                        }
                    },
                    new List<string>()
                }
            });
        });

        services.AddAuthentication()
            .AddJwtBearer("AccessToken", _ => {})
            .AddJwtBearer("FileAccessToken", _ => {});

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor;
        });

        services.AddAuthorization(options =>
        {
            var schemes = new[] { "AccessToken", "FileAccessToken" };
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(schemes)
                .Build();
        });

        // _
        services.Configure<QwereeConfigurationDo>(Configuration.GetSection("Qweree"));
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // Session
        services.AddScoped(p =>
            p.GetRequiredService<IHttpContextAccessor>().HttpContext?.User ?? new ClaimsPrincipal());
        services.AddScoped<ISessionStorage, ClaimsPrincipalStorage>();
        services.AddScoped<ClaimsPrincipalStorage, ClaimsPrincipalStorage>();

        // Database
        services.AddSingleton(p =>
        {
            var config = p.GetRequiredService<IOptions<QwereeConfigurationDo>>().Value;
            return new MongoContext(config.MongoConnectionString ?? "", config.DatabaseName ?? "");
        });

        // Http
        services.AddSingleton<HttpClientHandler>();
        services.AddSingleton(p =>
        {
            var httpHandler = p.GetRequiredService<HttpClientHandler>();
            var oauth2Client = new OAuth2Client(new HttpClient(httpHandler){BaseAddress = new Uri(new Uri(Configuration["Qweree:AuthUri"]!), "api/oauth2/")});
            var qwereeConfig = p.GetRequiredService<IOptions<QwereeConfigurationDo>>();
            var clientCredentials = new ClientCredentials
            {
                ClientId = qwereeConfig.Value.ClientId,
                ClientSecret = qwereeConfig.Value.ClientSecret
            };

            return new ClientCredentialsHandler(httpHandler, oauth2Client, clientCredentials, new MemoryTokenStorage());
        });
        services.AddScoped(p =>
        {

            var httpHandler = p.GetRequiredService<ClientCredentialsHandler>();
            var httpClient = new HttpClient(httpHandler)
            {
                BaseAddress = new Uri(Configuration["Qweree:CdnUri"]!)
            };
            return new StorageClient(httpClient);
        });

        // Picc stash
        services.AddScoped<StashedPiccRepository>();

        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();
        services.AddSingleton<FileAccessKey>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<QwereeConfigurationDo> config)
    {
        var pathBase = config.Value.PathBase;

        if (pathBase is not null)
            app.UsePathBase(pathBase);

        app.UseForwardedHeaders();
        app.UseSwagger(c =>
        {
            c.PreSerializeFilters.Add((swaggerDoc, _) =>
            {
                swaggerDoc.Servers = new List<OpenApiServer> { new(){ Url = $"{pathBase ?? "/"}" } };
            });
        });
        app.UseSwaggerUI(c => c.SwaggerEndpoint((pathBase ?? "") + "/swagger/v1/swagger.json", "Qweree Picc Stash api"));
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}