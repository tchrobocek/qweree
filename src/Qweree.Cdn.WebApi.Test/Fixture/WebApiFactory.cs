using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Cdn.WebApi.Infrastructure.Authentication;
using Qweree.Cdn.WebApi.Infrastructure.Storage;
using Qweree.Mongo;
using Qweree.TestUtils.IO;
using Qweree.Utils;

namespace Qweree.Cdn.WebApi.Test.Fixture;

public class WebApiFactory : WebApplicationFactory<Startup>
{
    protected override IHostBuilder CreateHostBuilder()
    {
        return base.CreateHostBuilder()!
            .ConfigureServices(ConfigureServices);
    }

    private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        var objectStorage = services.First(s => s.ServiceType == typeof(IObjectStorage));
        services.Remove(objectStorage);

        services.AddSingleton<TemporaryFolder>();
        services.AddSingleton<IObjectStorage>(p =>
        {
            var tempFolder = p.GetRequiredService<TemporaryFolder>();
            return new FileObjectStorage(tempFolder.Path);
        });

        services.AddSingleton(_ =>
            new MongoContext(Settings.Database.ConnectionString, Settings.Database.DatabaseName));
    }

    public async Task<HttpClient> CreateAuthenticatedClientAsync(PasswordGrantInput passwordInput, ClientCredentials clientCredentials)
    {
        var authConfig = Services.GetRequiredService<IOptions<AuthenticationConfigurationDo>>().Value;
        var client = CreateClient();
        var oauthClient = new HttpClient
        {
            BaseAddress = new Uri(authConfig.TokenUri!)
        };
        var oAuth2Client = new OAuth2Client(oauthClient);
        var response = await oAuth2Client.SignInAsync(passwordInput, clientCredentials);
        response.EnsureSuccessStatusCode();
        var token = await response.ReadPayloadAsync(JsonUtils.SnakeCaseNamingPolicy);
        client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token?.AccessToken}");
        return client;
    }
}