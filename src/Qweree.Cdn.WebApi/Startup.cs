using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Qweree.AspNet.Session;
using MongoDB.HealthCheck;
using Qweree.AspNet.Web.Swagger;
using Qweree.Cdn.WebApi.Application.Storage;
using Qweree.Cdn.WebApi.Domain.Storage;
using Qweree.Cdn.WebApi.Infrastructure.Authentication;
using Qweree.Cdn.WebApi.Infrastructure.Storage;
using Qweree.Mongo;
using Qweree.Utils;

namespace Qweree.Cdn.WebApi
{
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
            services.AddHealthChecks()
                .AddMongoHealthCheck("Database", Configuration["HealthChecks:Database:ConnectionString"]);

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
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
                    {new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2_password"
                        }
                    }, new List<string>()}
                });
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = Issuer,
                    ValidateAudience = true,
                    ValidAudience = Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Authentication:AccessTokenKey"])),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

            services.AddAuthorization();

            // _
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            // Session
            services.Configure<AuthenticationConfigurationDo>(Configuration.GetSection("Authentication"));
            services.AddScoped(p => p.GetRequiredService<IHttpContextAccessor>().HttpContext?.User ?? new ClaimsPrincipal());
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Qweree Cdn v1 api"));
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}