using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Qweree.Authentication.Sdk.Account;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.PiccStash.Sdk;
using Qweree.Sdk.Http.HttpClient;
using Qweree.WebApplication.Infrastructure.Authentication;
using Qweree.WebApplication.Infrastructure.Browser;
using Qweree.WebApplication.Infrastructure.ServicesOverview;

namespace Qweree.WebApplication
{
    public class Startup
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly IWebAssemblyHostEnvironment _hostEnvironment;

        public Startup(IWebAssemblyHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            services.AddOptions();
            services.AddAuthorizationCore();
            services.AddMudServices();
            services.AddSingleton<LocalStorage>();
            services.AddSingleton<LocalTokenStorage>();
            services.AddSingleton(p => new QwereeHttpHandler(new HttpClientHandler(),
                p.GetRequiredService<LocalTokenStorage>()));
            services.AddScoped<AuthenticationStateProvider, ApplicationAuthenticationStateProvider>();
            services.AddScoped<ClaimsPrincipalStorage>();
            services.AddScoped(p =>
            {
                return new UnauthorizedHttpHandler(p.GetRequiredService<AuthenticationService>(),
                    p.GetRequiredService<NavigationManager>(), p.GetRequiredService<QwereeHttpHandler>());
            });
            services.AddScoped(_ =>
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri(new Uri(configuration["TokenServiceUri"]), "api/oauth2/auth/")
                };
                return new OAuth2Client(client);
            });
            services.AddScoped(p =>
            {
                var client = new HttpClient(p.GetRequiredService<UnauthorizedHttpHandler>())
                {
                    BaseAddress = new Uri(new Uri(configuration["TokenServiceUri"]), "api/system/")
                };
                return new SystemInfoClient(client);
            });
            services.AddScoped(p =>
            {
                var client = new HttpClient(p.GetRequiredService<UnauthorizedHttpHandler>())
                {
                    BaseAddress = new Uri(new Uri(configuration["PiccServiceUri"]) , "api/v1/picc/")
                };
                return new PiccClient(client);
            });
            services.AddScoped(p =>
            {
                var client = new HttpClient(p.GetRequiredService<UnauthorizedHttpHandler>())
                {
                    BaseAddress = new Uri(new Uri(configuration["TokenServiceUri"]) , "api/account/")
                };
                return new MyAccountClient(client);
            });
            services.AddScoped<AuthenticationService>();
        }
    }
}