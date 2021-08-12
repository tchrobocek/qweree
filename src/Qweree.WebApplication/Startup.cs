using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Qweree.Authentication.Sdk.OAuth2;
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

        public void ConfigureServices(IServiceCollection services)
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
            services.AddScoped(_ =>
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri("http://localhost/auth/api/oauth2/auth/", UriKind.Absolute)
                };
                return new OAuth2Client(client);
            });
            services.AddScoped(p =>
            {
                var client = new HttpClient(p.GetRequiredService<QwereeHttpHandler>())
                {
                    BaseAddress = new Uri("http://localhost/auth/api/system/", UriKind.Absolute)
                };
                return new SystemInfoClient(client);
            });
            services.AddScoped<AuthenticationService>();
        }
    }
}