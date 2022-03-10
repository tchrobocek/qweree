using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.Http;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.ConsoleApplication.Infrastructure.Authentication;

namespace Qweree.ConsoleApplication.Infrastructure;

public class HttpMessageHandlerFactory
{
    private readonly HttpMessageHandler _innerHandler;
    private readonly ITokenStorage _tokenStorage;
    private readonly OAuth2ClientFactory _oauthClientFactory;

    public HttpMessageHandlerFactory(HttpMessageHandler innerHandler, ITokenStorage tokenStorage, OAuth2ClientFactory oauthClientFactory)
    {
        _innerHandler = innerHandler;
        _tokenStorage = tokenStorage;
        _oauthClientFactory = oauthClientFactory;
    }

    public async Task<HttpMessageHandler> CreateHandlerAsync(CancellationToken cancellationToken = new())
    {
        var authorizationHeaderHandler = new AuthorizationHeaderHandler(_innerHandler, _tokenStorage);
        var oauth2Client = await _oauthClientFactory.CreateClientAsync(cancellationToken);
        return new RefreshTokenHandler(authorizationHeaderHandler, _tokenStorage, oauth2Client,
            new ClientCredentials("admin-cli", "admin"));
    }
}