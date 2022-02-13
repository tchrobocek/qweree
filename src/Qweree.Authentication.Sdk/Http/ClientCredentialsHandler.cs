using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Utils;

namespace Qweree.Authentication.Sdk.Http;

public class ClientCredentialsHandler : DelegatingHandler
{
    private readonly OAuth2Client _oauthClient;
    private readonly ClientCredentials _clientCredentials;
    private readonly ITokenStorage _tokenStorage;

    public ClientCredentialsHandler(HttpMessageHandler innerHandler, OAuth2Client oauthClient, ClientCredentials clientCredentials, ITokenStorage tokenStorage)
        : base(innerHandler)
    {
        _oauthClient = oauthClient;
        _clientCredentials = clientCredentials;
        _tokenStorage = tokenStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        TokenInfo? tokenInfo = null;
        var authenticated = false;

        try
        {
            tokenInfo = await _tokenStorage.GetTokenInfoAsync(cancellationToken);
        }
        finally
        {
            if (tokenInfo is null)
            {
                authenticated = true;
                tokenInfo = await AuthenticateAsync(cancellationToken);
            }

            if (tokenInfo is not null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenInfo.AccessToken);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (tokenInfo is not null && !authenticated && response.StatusCode == HttpStatusCode.Unauthorized)
        {
            tokenInfo = await AuthenticateAsync(cancellationToken);

            if (tokenInfo is null)
                return response;

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenInfo.AccessToken);
            return await base.SendAsync(request, cancellationToken);
        }

        return response;
    }

    private async Task<TokenInfo?> AuthenticateAsync(CancellationToken cancellationToken = new())
    {
        var response = await _oauthClient.SignInAsync(_clientCredentials, cancellationToken);

        if (!response.IsSuccessful)
            return null;

        var dto = await response.ReadPayloadAsync(JsonUtils.SnakeCaseNamingPolicy, cancellationToken);
        if (dto is null)
            return null;

        var tokenInfo = TokenInfoMapper.FromDto(dto);
        await _tokenStorage.SetTokenInfoAsync(tokenInfo, cancellationToken);
        return tokenInfo;
    }
}