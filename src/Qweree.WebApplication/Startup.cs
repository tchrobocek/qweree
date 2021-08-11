using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Qweree.WebApplication.Infrastructure.Authentication;

namespace Qweree.WebApplication
{
    public class Startup
    {
        private readonly IWebAssemblyHostEnvironment _hostEnvironment;

        public Startup(IWebAssemblyHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddScoped(_ => new HttpClient {BaseAddress = new Uri(_hostEnvironment.BaseAddress)});
            services.AddOptions();
            services.AddAuthorizationCore();
            services.AddScoped<AuthenticationStateProvider, ApplicationAuthenticationStateProvider>();
            services.AddScoped<ClaimsPrincipalStorage>();

        }
    }
}