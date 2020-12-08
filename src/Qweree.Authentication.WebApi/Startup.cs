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
using MongoDB.HealthCheck;
using Qweree.AspNet.Session;
using Qweree.Authentication.WebApi.Application.Authentication;
using Qweree.Authentication.WebApi.Application.Identity;
using Qweree.Authentication.WebApi.Domain.Authentication;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Infrastructure;
using Qweree.Authentication.WebApi.Infrastructure.Authentication;
using Qweree.Authentication.WebApi.Infrastructure.Identity;
using Qweree.Authentication.WebApi.Infrastructure.Validations;
using Qweree.Mongo;
using Qweree.Utils;
using Qweree.Validator;
using Qweree.Validator.Extensions;
using Qweree.Validator.ModelValidation;

namespace Qweree.Authentication.WebApi
{
    public class Startup
    {
        public static TokenValidationParameters GetValidationParameters(string accessTokenKey)
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = AuthenticationService.Issuer,
                ValidateAudience = true,
                ValidAudience = AuthenticationService.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessTokenKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };
        }

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
                options.SwaggerDoc("v1", new OpenApiInfo {Title = "Qweree.Authentication.WebApi", Version = "v1"});
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
                            AuthorizationUrl = new Uri("/api/oauth2/auth", UriKind.Relative),
                            RefreshUrl = new Uri("/api/oauth2/auth", UriKind.Relative),
                            TokenUrl = new Uri("/api/oauth2/auth", UriKind.Relative)
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
                options.TokenValidationParameters =
                    GetValidationParameters(Configuration["Authentication:AccessTokenKey"]);
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("UserCreate", policy => policy.RequireClaim(ClaimTypes.Role, "UserUpdate"));
            });

            // Validator
            services.AddSingleton<IConstraintValidator, PasswordConstraintValidator>();
            services.AddSingleton<IConstraintValidator, UniqueConstraintValidator>();
            services.AddSingleton<IValidator>(p =>
            {
                var validatorBuilder = new ValidatorBuilder();
                validatorBuilder.WithModelValidator();
                validatorBuilder.WithStaticModelSettings(ValidationMap.ConfigureValidator);
                validatorBuilder.WithAttributeModelSettings(typeof(Program).Assembly);
                validatorBuilder.WithDefaultConstraints();

                var validators = p.GetServices<IConstraintValidator>();
                foreach (var validator in validators)
                    validatorBuilder.WithConstraintValidator(validator);

                return validatorBuilder.Build();
            });

            // _
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            // Database
            services.Configure<DatabaseConfigurationDo>(Configuration.GetSection("Database"));
            services.AddSingleton(p =>
            {
                var config = p.GetRequiredService<IOptions<DatabaseConfigurationDo>>().Value;
                return new MongoContext(config.ConnectionString ?? "", config.DatabaseName ?? "");
            });

            // Authentication
            services.Configure<AuthenticationConfigurationDo>(Configuration.GetSection("Authentication"));
            services.AddSingleton<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddSingleton(p =>
            {
                var userRepository = p.GetRequiredService<IUserRepository>();
                var refreshTokenRepository = p.GetRequiredService<IRefreshTokenRepository>();
                var dateTimeProvider = p.GetRequiredService<IDateTimeProvider>();
                var config = p.GetRequiredService<IOptions<AuthenticationConfigurationDo>>().Value;

                return new AuthenticationService(userRepository, refreshTokenRepository, dateTimeProvider, new Random(),
                    config.AccessTokenValiditySeconds ?? 0, config.RefreshTokenValiditySeconds ?? 0, config.AccessTokenKey ?? "", config.FileAccessTokenKey ?? "", config.FileAccessTokenValiditySeconds ?? 0);
            });

            // Identity
            services.AddScoped(p => p.GetRequiredService<HttpContext>().User);
            services.AddScoped<ISessionStorage, ClaimsPrincipalStorage>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IUniqueConstraintValidatorRepository, UserRepository>();
            services.AddSingleton(p => new UserService(p.GetRequiredService<IDateTimeProvider>(),
                p.GetRequiredService<IUserRepository>(), p.GetRequiredService<IValidator>()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Qweree OAuth2 v1 api"));
            app.UseCors("liberal");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}