using System;
using System.Collections.Generic;
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
using Qweree.Authentication.Sdk.Session;
using Qweree.Cdn.Sdk.Explorer;
using Qweree.Cdn.WebApi.Domain.Storage;
using Qweree.Cdn.WebApi.Infrastructure;
using Qweree.Cdn.WebApi.Infrastructure.Explorer;
using Qweree.Cdn.WebApi.Infrastructure.Storage;
using Qweree.Cdn.WebApi.Infrastructure.System;
using Qweree.Mongo;
using Qweree.Utils;

namespace Qweree.Cdn.WebApi;

public class Startup
{
    public const string Audience = "qweree";
    public const string Issuer = "net.qweree";

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddHealthChecks()
            .AddMongoHealthCheck("Database", Configuration["Qweree:HealthCheckConnectionString"]);

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.Converters.Add(new ExplorerObjectConverter());
            });
        services.AddSwaggerGen(options =>
        {
            options.OperationFilter<FileFromBodyOperationFilter>();
            options.SwaggerDoc("v1", new OpenApiInfo {Title = "Qweree.Cdn.WebApi", Version = "v1"});
            options.AddSecurityDefinition("oauth2_password", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.OAuth2,
                Scheme = "Bearer",
                Flows = new OpenApiOAuthFlows
                {
                    Password = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(Configuration["Qweree:SwaggerTokenUri"], UriKind.Absolute),
                        RefreshUrl = new Uri(Configuration["Qweree:SwaggerTokenUri"], UriKind.Absolute),
                        TokenUrl = new Uri(Configuration["Qweree:SwaggerTokenUri"], UriKind.Absolute)
                    }
                }
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2_password"
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

            options.AddPolicy("StorageStore", policy =>
            {
                policy.RequireClaim(ClaimTypes.Role, "qweree.cdn.storage.store");
                policy.AuthenticationSchemes = schemes;
            });
            options.AddPolicy("StorageStoreForce", policy =>
            {
                policy.RequireClaim(ClaimTypes.Role, "qweree.cdn.storage.store_force");
                policy.AuthenticationSchemes = schemes;
            });
            options.AddPolicy("StorageExplore", policy =>
            {
                policy.RequireClaim(ClaimTypes.Role, "qweree.cdn.storage.explore");
                policy.AuthenticationSchemes = schemes;
            });
            options.AddPolicy("StorageDelete", policy =>
            {
                policy.RequireClaim(ClaimTypes.Role, "qweree.cdn.storage.delete");
                policy.AuthenticationSchemes = schemes;
            });
        });

        // _
        services.Configure<QwereeConfigurationDo>(Configuration.GetSection("Qweree"));
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // Session
        services.AddScoped(p =>
            p.GetRequiredService<IHttpContextAccessor>().HttpContext?.User ?? new ClaimsPrincipal());
        services.AddScoped<ISessionStorage, ClaimsPrincipalStorage>();
        services.AddScoped<ClaimsPrincipalStorage, ClaimsPrincipalStorage>();
        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();
        services.AddSingleton<FileAccessKey>();

        // Database
        services.AddSingleton(p =>
        {
            var config = p.GetRequiredService<IOptions<QwereeConfigurationDo>>().Value;
            return new MongoContext(config.MongoConnectionString ?? "", config.DatabaseName ?? "");
        });

        // Storage
        services.AddScoped<IStoredObjectRepository, StoredObjectRepository>();
        services.AddScoped<IStoredObjectDescriptorRepository, StoredObjectDescriptorRepository>();
        services.AddScoped<IObjectStorage, FileObjectStorage>(p =>
        {
            var config = p.GetRequiredService<IOptions<QwereeConfigurationDo>>().Value;
            return new FileObjectStorage(config.FileSystemRoot!);
        });
        services.AddScoped<StoredObjectService>();
        services.AddScoped<ExplorerService>();
        services.AddScoped<StatsService>();
        // services.AddSingleton<IStorageBuffer, MemoryBuffer>();
        services.AddSingleton<IStorageBuffer>(p =>
        {
            var config = p.GetRequiredService<IOptions<QwereeConfigurationDo>>();
            return new TempFolderBuffer(config.Value.FileSystemTemp!);
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<QwereeConfigurationDo> config)
    {
        var pathBase = config.Value.PathBase;

        if (pathBase != null)
            app.UsePathBase(pathBase);

        app.UseForwardedHeaders();
        app.UseSwagger(c =>
        {
            c.PreSerializeFilters.Add((swaggerDoc, _) =>
            {
                swaggerDoc.Servers = new List<OpenApiServer> { new(){ Url = $"{pathBase ?? "/"}" } };
            });
        });
        app.UseSwaggerUI(c => c.SwaggerEndpoint((pathBase ?? "") + "/swagger/v1/swagger.json", "Qweree Cdn api"));
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}