using System;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Authentication.Sdk.Test.Fixture;
using Qweree.Utils;
using Xunit;

namespace Qweree.Authentication.Sdk.Test.OAuth2;

[Collection("Authentication adapter collection")]
[Trait("Category", "Integration test")]
public class OAuth2ClientTest : IClassFixture<AuthenticationAdapterFixture>
{
    private readonly OAuth2Client _oAuth2Client;

    public OAuth2ClientTest(AuthenticationAdapterFixture authFixture)
    {
        var uri = new Uri(authFixture.AuthenticationApiUri);
        var client = authFixture.CreateHttpClientAsync().GetAwaiter().GetResult();
        client.BaseAddress = new Uri(uri, "api/oauth2/auth");
        _oAuth2Client = new OAuth2Client(client);
    }

    [Fact]
    public async Task TestAuthenticatePassword()
    {
        var input = new PasswordGrantInput
        {
            Username = AuthenticationAdapterFixture.TestAdminUsername,
            Password = AuthenticationAdapterFixture.TestAdminPassword
        };
        var clientCredentials = new ClientCredentials
        {
            ClientId = AuthenticationAdapterFixture.TestClientId,
            ClientSecret = AuthenticationAdapterFixture.TestClientSecret
        };
        var response = await _oAuth2Client.SignInAsync(input, clientCredentials);

        response.EnsureSuccessStatusCode();
        var tokenInfo = await response.ReadPayloadAsync(JsonUtils.SnakeCaseNamingPolicy);

        Assert.NotEmpty(tokenInfo?.AccessToken ?? "");
        Assert.NotEmpty(tokenInfo?.RefreshToken ?? "");
    }
}