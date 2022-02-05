using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Qweree.AspNet.Configuration;
using Qweree.AspNet.Session;
using Qweree.AspNet.Web.Swagger;
using Qweree.Cdn.Sdk.Explorer;
using Qweree.Cdn.WebApi.Domain.Storage;
using Qweree.Cdn.WebApi.Infrastructure.Authentication;
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

    public static TokenValidationParameters GetValidationParameters(string accessTokenKey)
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = Issuer,
            ValidateAudience = true,
            ValidAudience = Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessTokenKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddHealthChecks()
            .AddMongoHealthCheck("Database", Configuration["HealthChecks:Database:ConnectionString"]);

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
                        AuthorizationUrl = new Uri(Configuration["Swagger:TokenUri"], UriKind.Absolute),
                        RefreshUrl = new Uri(Configuration["Swagger:TokenUri"], UriKind.Absolute),
                        TokenUrl = new Uri(Configuration["Swagger:TokenUri"], UriKind.Absolute)
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

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters =
                GetValidationParameters(Configuration["Authentication:AccessTokenKey"]);
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    string? token;

                    if (context.Request.Headers.TryGetValue(HeaderNames.Authorization, out var values))
                    {
                        context.Options.TokenValidationParameters =
                            GetValidationParameters(Configuration["Authentication:AccessTokenKey"]);

                        token = values.FirstOrDefault();

                        if (token == null)
                            return Task.CompletedTask;

                        const string prefix = "Bearer ";

                        if (token.StartsWith(prefix))
                            token = token.Substring(prefix.Length);
                        else
                            token = null;
                    }
                    else if (context.Request.Query.TryGetValue("access_token", out values))
                    {
                        token = values.FirstOrDefault();
                    }
                    else
                    {
                        return Task.CompletedTask;
                    }

                    context.Token = token;
                    return Task.CompletedTask;
                }
            };
        });
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("StorageStore", policy => policy.RequireClaim(ClaimTypes.Role, "qweree.cdn.storage.store"));
            options.AddPolicy("StorageStoreForce", policy => policy.RequireClaim(ClaimTypes.Role, "qweree.cdn.storage.store_force"));
            options.AddPolicy("StorageExplore", policy => policy.RequireClaim(ClaimTypes.Role, "qweree.cdn.storage.explore"));
            options.AddPolicy("StorageDelete", policy => policy.RequireClaim(ClaimTypes.Role, "qweree.cdn.storage.delete"));
        });

        // _
        services.Configure<RoutingConfigurationDo>(Configuration.GetSection("Routing"));
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // Session
        services.Configure<AuthenticationConfigurationDo>(Configuration.GetSection("Authentication"));
        services.AddScoped(p =>
            p.GetRequiredService<IHttpContextAccessor>().HttpContext?.User ?? new ClaimsPrincipal());
        services.AddScoped<ISessionStorage, ClaimsPrincipalStorage>();
        services.AddScoped<ClaimsPrincipalStorage, ClaimsPrincipalStorage>();

        // Database
        services.Configure<DatabaseConfigurationDo>(Configuration.GetSection("Database"));
        services.AddSingleton(p =>
        {
            var config = p.GetRequiredService<IOptions<DatabaseConfigurationDo>>().Value;
            return new MongoContext(config.ConnectionString ?? "", config.DatabaseName ?? "");
        });

        // Storage
        services.Configure<StorageConfigurationDo>(Configuration.GetSection("Storage"));
        services.AddScoped<IStoredObjectRepository, StoredObjectRepository>();
        services.AddScoped<IStoredObjectDescriptorRepository, StoredObjectDescriptorRepository>();
        services.AddScoped<IObjectStorage, FileObjectStorage>(p =>
        {
            var config = p.GetRequiredService<IOptions<StorageConfigurationDo>>().Value;
            return new FileObjectStorage(config.FileSystemRoot!);
        });
        services.AddScoped<StoredObjectService>();
        services.AddScoped<ExplorerService>();
        services.AddScoped<StatsService>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<RoutingConfigurationDo> routingConfiguration)
    {
        var pathBase = routingConfiguration.Value.PathBase;

        if (pathBase != null)
        {
            app.UsePathBase(pathBase);
        }
        app.UseForwardedHeaders();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint((pathBase ?? "") + "/swagger/v1/swagger.json", "Qweree Cdn api"));
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}