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
using Qweree.Authentication.Sdk.Session;
using Qweree.Authentication.Sdk.Session.Tokens;
using Qweree.Authentication.Sdk.Session.Tokens.Jwt;
using Qweree.Authentication.WebApi.Domain;
using Qweree.Authentication.WebApi.Domain.Account;
using Qweree.Authentication.WebApi.Domain.Authentication;
using Qweree.Authentication.WebApi.Domain.Authorization;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Domain.Identity.UserInvitation;
using Qweree.Authentication.WebApi.Domain.Security;
using Qweree.Authentication.WebApi.Infrastructure;
using Qweree.Authentication.WebApi.Infrastructure.Authentication;
using Qweree.Authentication.WebApi.Infrastructure.Authorization.Roles;
using Qweree.Authentication.WebApi.Infrastructure.Identity;
using Qweree.Authentication.WebApi.Infrastructure.Identity.UserInvitation;
using Qweree.Authentication.WebApi.Infrastructure.Security;
using Qweree.Authentication.WebApi.Infrastructure.Validations;
using Qweree.Mongo;
using Qweree.Utils;
using Qweree.Validator;
using Qweree.Validator.Extensions;
using Qweree.Validator.ModelValidation;

namespace Qweree.Authentication.WebApi;

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
            .AddMongoHealthCheck("Database", Configuration["Qweree:HealthCheckConnectionString"]);
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

        var pathBase = Configuration["Qweree:PathBase"];
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
                GetValidationParameters(Configuration["Qweree:AccessTokenKey"]);
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
            options.AddPolicy("UserInvitationCreate", policy => policy.RequireClaim(ClaimTypes.Role, "qweree.auth.userInvitations.create"));
            options.AddPolicy("UserInvitationRead", policy => policy.RequireClaim(ClaimTypes.Role, "qweree.auth.userInvitations.read"));
            options.AddPolicy("UserInvitationDelete", policy => policy.RequireClaim(ClaimTypes.Role, "qweree.auth.userInvitations.delete"));
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
        services.Configure<QwereeConfigurationDo>(Configuration.GetSection("Qweree"));
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // Database
        services.AddSingleton(p =>
        {
            var config = p.GetRequiredService<IOptions<QwereeConfigurationDo>>().Value;
            return new MongoContext(config.MongoConnectionString ?? "", config.DatabaseName ?? "");
        });

        // Authentication
        services.AddSingleton<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddSingleton(p =>
        {
            var userRepository = p.GetRequiredService<IUserRepository>();
            var refreshTokenRepository = p.GetRequiredService<IRefreshTokenRepository>();
            var dateTimeProvider = p.GetRequiredService<IDateTimeProvider>();
            var config = p.GetRequiredService<IOptions<QwereeConfigurationDo>>().Value;
            var passwordEncoder = p.GetRequiredService<IPasswordEncoder>();
            var clientRepository = p.GetRequiredService<IClientRepository>();
            var authorizationService = p.GetRequiredService<AuthorizationService>();
            var clientRoleRepository = p.GetRequiredService<IClientRoleRepository>();
            var tokenEncoder = p.GetRequiredService<ITokenEncoder>();

            return new AuthenticationService(userRepository, refreshTokenRepository, dateTimeProvider, new Random(),
                config.AccessTokenValiditySeconds ?? 0, config.RefreshTokenValiditySeconds ?? 0, passwordEncoder, clientRepository, authorizationService, clientRoleRepository, tokenEncoder);
        });
        services.AddSingleton<ITokenEncoder>(p =>
        {
            var config = p.GetRequiredService<IOptions<QwereeConfigurationDo>>().Value;
            return new JwtEncoder(Issuer, Audience, config?.AccessTokenKey ?? string.Empty);
        });

        // Identity
        services.AddScoped(p =>
            p.GetRequiredService<IHttpContextAccessor>().HttpContext?.User ?? new ClaimsPrincipal());
        services.AddScoped<ISessionStorage, ClaimsPrincipalStorage>();
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IUserInvitationRepository, UserInvitationRepository>();
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
        services.AddSingleton<UserRegisterService>();
        services.AddScoped(p => new UserService(p.GetRequiredService<IUserRepository>(),
            p.GetRequiredService<ISessionStorage>(), p.GetRequiredService<SdkMapperService>(),
            p.GetRequiredService<AuthorizationService>()));
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
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<QwereeConfigurationDo> qwereeConfig)
    {
        var pathBase = qwereeConfig.Value.PathBase;

        if (pathBase != null)
            app.UsePathBase(pathBase);

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All
        });
        app.UseSwagger(c =>
        {
            c.PreSerializeFilters.Add((swaggerDoc, _) =>
            {
                swaggerDoc.Servers = new List<OpenApiServer> { new(){ Url = $"{pathBase ?? "/"}" } };
            });
        });
        app.UseSwaggerUI(c => c.SwaggerEndpoint((pathBase ?? "") + "/swagger/v1/swagger.json", "Qweree OAuth2 api"));
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}