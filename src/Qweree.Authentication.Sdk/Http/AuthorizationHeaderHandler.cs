using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.OAuth2;

namespace Qweree.Authentication.Sdk.Http;

public class AuthorizationHeaderHandler : DelegatingHandler
{
    private readonly ITokenStorage _tokenStorage;

    public AuthorizationHeaderHandler(HttpMessageHandler innerHandler, ITokenStorage tokenStorage)
        : base(innerHandler)
    {
        _tokenStorage = tokenStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        TokenInfo? token;

        try
        {
            token = await _tokenStorage.GetTokenInfoAsync(cancellationToken);
        }
        catch (Exception)
        {
            token = null;
        }

        if (token is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        return await base.SendAsync(request, cancellationToken);
    }
}