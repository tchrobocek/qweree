using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Qweree.Authentication.AdminSdk.Authorization;
using Qweree.Authentication.AdminSdk.Identity;
using Qweree.Authentication.Sdk.Account;
using Qweree.Cdn.Sdk.Explorer;
using Qweree.Cdn.Sdk.Storage;
using Qweree.Cdn.Sdk.System;
using Qweree.Gateway.Sdk;
using Qweree.PiccStash.Sdk;
using Qweree.WebApplication.Infrastructure.Authentication;
using Qweree.WebApplication.Infrastructure.Browser;
using Qweree.WebApplication.Infrastructure.Notes;
using Qweree.WebApplication.Infrastructure.ServicesOverview;
using Qweree.WebApplication.Infrastructure.View;

namespace Qweree.WebApplication;

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
        services.AddSingleton<WindowService>();
        services.AddSingleton<LocalStorage>();
        services.AddSingleton<LocalUserStorage>();
        services.AddScoped<AuthenticationStateProvider, ApplicationAuthenticationStateProvider>();
        services.AddScoped<ClaimsPrincipalStorage>();
        services.AddScoped<HttpMessageHandler, HttpClientHandler>();
        services.AddScoped<BrowserCredentialsHandler>();
        services.AddScoped(p =>
        {
            return new UnauthorizedHttpHandler(p.GetRequiredService<AuthenticationService>(), p.GetRequiredService<NavigationManager>(), p.GetRequiredService<BrowserCredentialsHandler>());
        });
        services.AddScoped(p =>
        {
            var client = new HttpClient(p.GetRequiredService<BrowserCredentialsHandler>())
            {
                BaseAddress = new Uri(new Uri(configuration["GatewayServiceUri"]), "api/v1/auth/")
            };
            return new AuthenticationClient(client);
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
        services.AddScoped(p =>
        {
            var client = new HttpClient(p.GetRequiredService<UnauthorizedHttpHandler>())
            {
                BaseAddress = new Uri(new Uri(configuration["TokenServiceUri"]) , "api/admin/authorization/")
            };
            return new AuthorizationClient(client);
        });
        services.AddScoped(p =>
        {
            var client = new HttpClient(p.GetRequiredService<UnauthorizedHttpHandler>())
            {
                BaseAddress = new Uri(new Uri(configuration["TokenServiceUri"]) , "api/admin/identity/")
            };
            return new IdentityClient(client);
        });
        services.AddScoped(p =>
        {
            var client = new HttpClient(p.GetRequiredService<UnauthorizedHttpHandler>())
            {
                BaseAddress = new Uri(new Uri(configuration["CdnServiceUri"]) , "api/system/stats/")
            };
            return new StatsClient(client);
        });
        services.AddScoped(p =>
        {
            var client = new HttpClient(p.GetRequiredService<UnauthorizedHttpHandler>())
            {
                BaseAddress = new Uri(new Uri(configuration["CdnServiceUri"]) , "api/v1/explorer/")
            };
            return new ExplorerClient(client);
        });
        services.AddScoped(p =>
        {
            var client = new HttpClient(p.GetRequiredService<UnauthorizedHttpHandler>())
            {
                BaseAddress = new Uri(new Uri(configuration["CdnServiceUri"]) , "api/v1/storage/")
            };
            return new StorageClient(client);
        });
        services.AddScoped<AuthenticationService>();
        services.AddScoped<SystemInfoClientFactory>();
        services.AddScoped<NoteService>();
        services.AddScoped<DialogService>();
    }
}