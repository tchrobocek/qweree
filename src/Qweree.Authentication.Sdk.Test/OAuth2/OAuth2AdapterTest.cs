using System;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Authentication.Sdk.Test.Fixture;
using Xunit;

namespace Qweree.Authentication.Sdk.Test.OAuth2
{
    [Collection("Authentication adapter collection")]
    [Trait("Category", "Integration test")]
    public class OAuth2AdapterTest : IClassFixture<AuthenticationAdapterFixture>
    {
        private readonly OAuth2Adapter _oAuth2Adapter;

        public OAuth2AdapterTest(AuthenticationAdapterFixture authFixture)
        {
            var uri = new Uri(authFixture.AuthenticationApiUri);
            _oAuth2Adapter = new OAuth2Adapter(new Uri(uri, "/api/oauth2/auth"),
                authFixture.CreateHttpClientAsync().GetAwaiter().GetResult());
        }

        [Fact]
        public async Task TestAuthenticatePassword()
        {
            var input = new PasswordGrantInput(AuthenticationAdapterFixture.TestAdminUsername,
                AuthenticationAdapterFixture.TestAdminPassword);
            var clientCredentials = new ClientCredentials(AuthenticationAdapterFixture.TestClientId,
                AuthenticationAdapterFixture.TestClientSecret);
            var tokenInfo = await _oAuth2Adapter.SignInAsync(input, clientCredentials);

            Assert.NotEmpty(tokenInfo.AccessToken);
            Assert.NotEmpty(tokenInfo.RefreshToken ?? "");
        }
    }
}