using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.OAuth2;

namespace Qweree.Authentication.Sdk.Test.Fixture;

public class AuthenticationAdapterFixture : IDisposable
{
    public const string TestAdminUsername = "admin";
    public const string TestAdminPassword = "password";
    public const string TestClientId = "test-cli";
    public const string TestClientSecret = "password";

    private readonly HttpMessageHandler _messageHandler;

    public AuthenticationAdapterFixture()
    {
        _messageHandler = new HttpClientHandler();
    }

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
        client.BaseAddress = new Uri(AuthenticationApiUri);
        var authAdapter = new OAuth2Client(client);
        var response = await authAdapter.SignInAsync(passwordGrantInput, clientCredentials, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();
        var token = await response.ReadPayloadAsync(cancellationToken);
        client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token?.AccessToken}");

        return client;
    }
}