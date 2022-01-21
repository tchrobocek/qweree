using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Utils;

namespace Qweree.Cdn.Sdk.Test.Fixture;

public class CdnAdapterFixture : IDisposable
{
    public const string TestAdminUsername = "admin";
    public const string TestAdminPassword = "password";
    public const string TestClientId = "test-cli";
    public const string TestClientSecret = "password";

    private readonly HttpMessageHandler _messageHandler;

    public CdnAdapterFixture()
    {
        _messageHandler = new HttpClientHandler();
    }

    public string CdnApiUri => "http://localhost/cdn/";
    public string AuthenticationApiUri => "http://localhost/auth/";

    public void Dispose()
    {
        _messageHandler.Dispose();
    }

    public Task<HttpClient> CreateHttpClientAsync(
        CancellationToken cancellationToken = new())
    {
        return Task.FromResult(new HttpClient(_messageHandler));
    }


    public async Task<HttpClient> CreateAuthenticatedHttpClientAsync(
        PasswordGrantInput passwordGrantInput,
        ClientCredentials clientCredentials,
        CancellationToken cancellationToken = new())
    {
        var client = await CreateHttpClientAsync(cancellationToken);
        client.BaseAddress = new Uri(new Uri(AuthenticationApiUri), "api/oauth2/auth");
        var oAuth2Client = new OAuth2Client(client);
        var response = await oAuth2Client.SignInAsync(passwordGrantInput, clientCredentials, cancellationToken);
        response.EnsureSuccessStatusCode();
        var token = await response.ReadPayloadAsync(JsonUtils.SnakeCaseNamingPolicy, cancellationToken);

        client = await CreateHttpClientAsync(cancellationToken);
        client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token?.AccessToken}");

        return client;
    }
}