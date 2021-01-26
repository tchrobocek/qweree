using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.Authentication;

namespace Qweree.Authentication.Sdk.Test.Fixture
{
    public class AuthenticationAdapterFixture : IDisposable
    {
        public const string TestAdminUsername = "admin";
        public const string TestAdminPassword = "password";

        private readonly HttpMessageHandler _messageHandler;

        public AuthenticationAdapterFixture()
        {
            _messageHandler = new HttpClientHandler();
        }

        public string AuthenticationApiUri => "http://localhost:10001";

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
            CancellationToken cancellationToken = new())
        {
            var client = await CreateHttpClientAsync(cancellationToken);
            var authAdapter = new OAuth2Adapter(new Uri(AuthenticationApiUri), client);
            var token = await authAdapter.SignInAsync(passwordGrantInput, cancellationToken);
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token.AccessToken}");

            return client;
        }
    }
}