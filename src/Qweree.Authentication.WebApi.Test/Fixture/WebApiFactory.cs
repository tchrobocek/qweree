using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Authentication.WebApi.Domain.Security;
using Qweree.Mongo;
using Qweree.Utils;

namespace Qweree.Authentication.WebApi.Test.Fixture;

public class WebApiFactory : WebApplicationFactory<Startup>
{
    private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        var contextService = services.FirstOrDefault(s => s.ServiceType == typeof(MongoContext));
        services.Remove(contextService!);

        services.AddSingleton(_ =>
            new MongoContext(Settings.Database.ConnectionString, Settings.Database.DatabaseName));
    }

    protected override IHostBuilder CreateHostBuilder()
    {
        return base.CreateHostBuilder()!
            .ConfigureServices(ConfigureServices);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<IPasswordEncoder, NonePasswordEncoder>();
        });
        base.ConfigureWebHost(builder);
    }

    public async Task<HttpClient> CreateAuthenticatedClientAsync(ClientCredentials clientCredentials, PasswordGrantInput passwordInput)
    {
        var client = CreateClient();
        var oauthClient = CreateClient();
        oauthClient.BaseAddress = new Uri(client.BaseAddress ?? new Uri("/"), "/api/oauth2/auth");
        var oAuth2Client = new OAuth2Client(oauthClient);
        var response = await oAuth2Client.SignInAsync(passwordInput, clientCredentials);
        response.EnsureSuccessStatusCode();
        var token = await response.ReadPayloadAsync(JsonUtils.SnakeCaseNamingPolicy);
        client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token?.AccessToken}");
        return client;
    }
}