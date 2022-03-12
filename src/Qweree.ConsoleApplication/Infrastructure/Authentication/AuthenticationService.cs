using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.ConsoleApplication.Infrastructure.RunContext;
using Qweree.Utils;

namespace Qweree.ConsoleApplication.Infrastructure.Authentication;

public class AuthenticationService
{
    private readonly OAuth2ClientFactory _oauthClientFactory;
    private readonly Context _context;

    public AuthenticationService(OAuth2ClientFactory oauthClientFactory, Context context)
    {
        _oauthClientFactory = oauthClientFactory;
        _context = context;
    }

    public async Task AuthenticateAsync(PasswordGrantInput passwordGrantInput, CancellationToken cancellationToken = new())
    {
        var oauthClient = await _oauthClientFactory.CreateClientAsync(cancellationToken);

        var clientCredentials = new ClientCredentials("admin-cli", "password");
        var response = await oauthClient.SignInAsync(passwordGrantInput, clientCredentials, cancellationToken: cancellationToken);

        response.EnsureSuccessStatusCode();

        var token = await response.ReadPayloadAsync(JsonUtils.SnakeCaseNamingPolicy, cancellationToken);
        await _context.SetCredentialsAsync(TokenInfoMapper.FromDto(token!), cancellationToken);
    }

    public async Task AuthenticateAsync(RefreshTokenGrantInput refreshTokenGrantInput, CancellationToken cancellationToken = new())
    {
        var oauthClient = await _oauthClientFactory.CreateClientAsync(cancellationToken);

        var clientCredentials = new ClientCredentials("admin-cli", "password");
        var response = await oauthClient.RefreshAsync(refreshTokenGrantInput, clientCredentials, cancellationToken: cancellationToken);

        response.EnsureSuccessStatusCode();

        var token = await response.ReadPayloadAsync(JsonUtils.SnakeCaseNamingPolicy, cancellationToken);
        await _context.SetCredentialsAsync(TokenInfoMapper.FromDto(token!), cancellationToken);
    }
}