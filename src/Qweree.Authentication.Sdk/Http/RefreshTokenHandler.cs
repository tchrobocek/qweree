using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.OAuth2;

namespace Qweree.Authentication.Sdk.Http;

public class RefreshTokenHandler : DelegatingHandler
{
    private readonly ITokenStorage _tokenStorage;
    private readonly OAuth2Client _oauthClient;
    private readonly ClientCredentials _clientCredentials;

    public RefreshTokenHandler(HttpMessageHandler innerHandler, ITokenStorage tokenStorage, OAuth2Client oauthClient, ClientCredentials clientCredentials)
        : base(innerHandler)
    {
        _tokenStorage = tokenStorage;
        _oauthClient = oauthClient;
        _clientCredentials = clientCredentials;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return response;
        }

        TokenInfo? token;

        try
        {
            token = await _tokenStorage.GetTokenInfoAsync(cancellationToken);
        }
        catch (Exception)
        {
            token = null;
        }

        if (token?.RefreshToken is null)
            return response;

        var input = new RefreshTokenGrantInput
        {
            RefreshToken = token.RefreshToken
        };
        var credentials = new ClientCredentials
        {
            ClientId = _clientCredentials.ClientId,
            ClientSecret = _clientCredentials.ClientSecret
        };
        var refreshResponse = await _oauthClient.RefreshAsync(input, credentials, cancellationToken: cancellationToken);

        if (!refreshResponse.IsSuccessful)
            return response;

        var payload = await refreshResponse.ReadPayloadAsync(cancellationToken);

        if (payload is null)
            return response;

        try
        {
            await _tokenStorage.SetTokenInfoAsync(payload, cancellationToken);
        }
        catch (Exception)
        {
            return response;
        }

        return await base.SendAsync(request, cancellationToken);
    }
}