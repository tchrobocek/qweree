using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.WebApplication.Infrastructure.Authentication;
using Qweree.WebApplication.Infrastructure.Browser;

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
            services.AddScoped<AuthenticationStateProvider, ApplicationAuthenticationStateProvider>();
            services.AddScoped<ClaimsPrincipalStorage>();
            services.AddScoped(_ =>
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri("http://localhost/auth/api/oauth2/auth", UriKind.Absolute)
                };
                return new OAuth2Client(client);
            });
            services.AddScoped<AuthenticationService>();
            services.AddScoped<LocalStorage>();
        }
    }
}