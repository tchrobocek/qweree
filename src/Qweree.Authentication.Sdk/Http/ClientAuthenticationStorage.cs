using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Utils;

namespace Qweree.Authentication.Sdk.Http;

public class ClientAuthenticationStorage : ITokenStorage
{
    private readonly ClientCredentials _clientCredentials;
    private readonly OAuth2Client _oauthClient;

    public ClientAuthenticationStorage(ClientCredentials clientCredentials, OAuth2Client oauthClient)
    {
        _clientCredentials = clientCredentials;
        _oauthClient = oauthClient;
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = new())
    {
        var response = await _oauthClient.SignInAsync(_clientCredentials, cancellationToken);
        response.EnsureSuccessStatusCode();

        var tokenInfo = await response.ReadPayloadAsync(JsonUtils.SnakeCaseNamingPolicy, cancellationToken);
        return tokenInfo?.AccessToken ?? string.Empty;
    }
}