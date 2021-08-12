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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using MongoDB.HealthCheck;
using Qweree.AspNet.Configuration;
using Qweree.AspNet.Session;
using Qweree.AspNet.Web.Swagger;
using Qweree.Mongo;
using Qweree.PiccStash.WebApi.Infrastructure.Authentication;
using Qweree.Utils;

namespace Qweree.PiccStash.WebApi
{
    public class Startup
    {
        public const string Audience = "qweree";
        public const string Issuer = "net.qweree";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static TokenValidationParameters GetValidationParameters(string accessTokenKey)
        {
            return new()
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
            services.AddCors(options =>
            {
                options.AddPolicy("liberal", builder =>
                {
                    builder.AllowAnyHeader()
                        .AllowAnyHeader()
                        .AllowAnyOrigin();
                });
            });
            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<FileFromBodyOperationFilter>();
                options.SwaggerDoc("v1", new OpenApiInfo {Title = "Qweree.PiccStash.WebApi", Version = "v1"});
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

            services.AddAuthorization();

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

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Qweree.PiccStash.WebApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}