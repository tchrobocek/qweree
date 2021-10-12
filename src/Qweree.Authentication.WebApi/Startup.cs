using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.HealthCheck;
using Qweree.AspNet.Configuration;
using Qweree.AspNet.Session;
using Qweree.Authentication.WebApi.Domain;
using Qweree.Authentication.WebApi.Domain.Account;
using Qweree.Authentication.WebApi.Domain.Authentication;
using Qweree.Authentication.WebApi.Domain.Authorization;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Domain.Security;
using Qweree.Authentication.WebApi.Infrastructure;
using Qweree.Authentication.WebApi.Infrastructure.Authentication;
using Qweree.Authentication.WebApi.Infrastructure.Authorization.Roles;
using Qweree.Authentication.WebApi.Infrastructure.Identity;
using Qweree.Authentication.WebApi.Infrastructure.Identity.UserRegister;
using Qweree.Authentication.WebApi.Infrastructure.Security;
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
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public static TokenValidationParameters GetValidationParameters(string accessTokenKey)
        {
            return new()
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
                });
            services.AddCors(options =>
            {
                options.AddPolicy("liberal", builder =>
                {
                    builder.AllowAnyHeader()
                        .AllowAnyHeader()
                        .AllowAnyOrigin()
                        .AllowAnyMethod();
                });
            });

            var pathBase = Configuration["Routing:PathBase"];
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
                            AuthorizationUrl = new Uri((pathBase ?? "") + "/api/oauth2/auth", UriKind.Relative),
                            RefreshUrl = new Uri((pathBase ?? "") + "/api/oauth2/auth", UriKind.Relative),
                            TokenUrl = new Uri((pathBase ?? "") + "/api/oauth2/auth", UriKind.Relative)
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
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("UserCreate", policy => policy.RequireClaim(ClaimTypes.Role, "qweree.auth.users.create"));
                options.AddPolicy("UserRead", policy => policy.RequireClaim(ClaimTypes.Role, "qweree.auth.users.read"));
                options.AddPolicy("UserDelete", policy => policy.RequireClaim(ClaimTypes.Role, "qweree.auth.users.delete"));
                options.AddPolicy("UserReadPersonalDetail",
                    policy => policy.RequireClaim(ClaimTypes.Role, "qweree.auth.users.read_personal"));
                options.AddPolicy("ClientCreate", policy => policy.RequireClaim(ClaimTypes.Role, "qweree.auth.clients.create"));
                options.AddPolicy("ClientRead", policy => policy.RequireClaim(ClaimTypes.Role, "qweree.auth.clients.read"));
                options.AddPolicy("ClientDelete", policy => policy.RequireClaim(ClaimTypes.Role, "qweree.auth.clients.delete"));
                options.AddPolicy("RoleCreate", policy => policy.RequireClaim(ClaimTypes.Role, "qweree.auth.roles.create"));
                options.AddPolicy("RoleRead", policy => policy.RequireClaim(ClaimTypes.Role, "qweree.auth.roles.read"));
                options.AddPolicy("RoleDelete", policy => policy.RequireClaim(ClaimTypes.Role, "qweree.auth.roles.delete"));
                options.AddPolicy("RoleModify", policy => policy.RequireClaim(ClaimTypes.Role, "qweree.auth.roles.modify"));
            });
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            // Validator
            services.AddSingleton<IConstraintValidator, PasswordConstraintValidator>();
            services.AddSingleton<IConstraintValidator, UniqueConstraintValidator>();
            services.AddSingleton<IConstraintValidator, ExistsConstraintValidator>();
            services.AddSingleton<CreateUserRoleValidator>();
            services.AddSingleton<ModifyUserRoleValidator>();
            services.AddSingleton<CreateClientRoleValidator>();
            services.AddSingleton<ModifyClientRoleValidator>();
            services.AddSingleton<IValidator>(p =>
            {
                var validatorBuilder = new ValidatorBuilder();
                validatorBuilder.WithModelValidator();
                validatorBuilder.WithStaticModelSettings(ValidationMap.ConfigureValidator);
                validatorBuilder.WithAttributeModelSettings(typeof(Program).Assembly);
                validatorBuilder.WithDefaultConstraints();
                validatorBuilder.WithObjectValidator(p.GetRequiredService<CreateUserRoleValidator>());
                validatorBuilder.WithObjectValidator(p.GetRequiredService<ModifyUserRoleValidator>());
                validatorBuilder.WithObjectValidator(p.GetRequiredService<CreateClientRoleValidator>());
                validatorBuilder.WithObjectValidator(p.GetRequiredService<ModifyClientRoleValidator>());

                var validators = p.GetServices<IConstraintValidator>();
                foreach (var validator in validators)
                    validatorBuilder.WithConstraintValidator(validator);

                return validatorBuilder.Build();
            });

            // _
            services.Configure<RoutingConfigurationDo>(Configuration.GetSection("Routing"));
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
                var passwordEncoder = p.GetRequiredService<IPasswordEncoder>();
                var clientRepository = p.GetRequiredService<IClientRepository>();
                var authorizationService = p.GetRequiredService<AuthorizationService>();
                var clientRoleRepository = p.GetRequiredService<IClientRoleRepository>();

                return new AuthenticationService(userRepository, refreshTokenRepository, dateTimeProvider, new Random(),
                    config.AccessTokenValiditySeconds ?? 0, config.RefreshTokenValiditySeconds ?? 0,
                    config.AccessTokenKey ?? "", passwordEncoder, clientRepository, authorizationService, clientRoleRepository);
            });

            // Identity
            services.AddScoped(p =>
                p.GetRequiredService<IHttpContextAccessor>().HttpContext?.User ?? new ClaimsPrincipal());
            services.AddScoped<ISessionStorage, ClaimsPrincipalStorage>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<UserInvitationRepository>();
            services.AddSingleton<IClientRepository, ClientRepository>();
            services.AddSingleton<IUniqueConstraintValidatorRepository, UserRepository>();
            services.AddSingleton<IUniqueConstraintValidatorRepository, UserRoleRepository>();
            services.AddSingleton<IUniqueConstraintValidatorRepository, ClientRepository>();
            services.AddSingleton<IUniqueConstraintValidatorRepository, ClientRoleRepository>();
            services.AddSingleton<IExistsConstraintValidatorRepository, UserRepository>();
            services.AddSingleton<IExistsConstraintValidatorRepository, UserRoleRepository>();
            services.AddSingleton<IExistsConstraintValidatorRepository, ClientRoleRepository>();
            services.AddSingleton<SdkMapperService, SdkMapperService>();
            services.AddSingleton<UserInvitationService>();
            services.AddSingleton<UserRegistrationService>();
            services.AddScoped(p => new UserService(p.GetRequiredService<IUserRepository>(),
                p.GetRequiredService<ISessionStorage>(),
                p.GetRequiredService<SdkMapperService>()));
            services.AddScoped(p => new ClientService(p.GetRequiredService<IValidator>(),
                p.GetRequiredService<IPasswordEncoder>(), p.GetRequiredService<IDateTimeProvider>(),
                p.GetRequiredService<IClientRepository>(), p.GetRequiredService<SdkMapperService>(),
                new Random()));

            // Authorization
            services.AddSingleton<IUserRoleRepository, UserRoleRepository>();
            services.AddSingleton<IClientRoleRepository, ClientRoleRepository>();
            services.AddSingleton<RoleService, RoleService>();
            services.AddSingleton<AuthorizationService>();
            services.AddScoped<MyAccountService>();

            // Security
            services.AddSingleton<IPasswordEncoder, BCryptPasswordEncoder>();
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
            app.UseSwaggerUI(c => c.SwaggerEndpoint((pathBase ?? "") + "/swagger/v1/swagger.json", "Qweree OAuth2 api"));
            app.UseCors("liberal");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}