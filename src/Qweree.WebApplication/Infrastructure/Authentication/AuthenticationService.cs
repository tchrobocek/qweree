using System.Threading;
using System.Threading.Tasks;
using Qweree.Gateway.Sdk;
using Qweree.WebApplication.Infrastructure.Browser;

namespace Qweree.WebApplication.Infrastructure.Authentication;

public class AuthenticationService
{
    private readonly AuthenticationClient _authenticationClient;
    private readonly LocalUserStorage _localUserStorage;

    public AuthenticationService(AuthenticationClient authenticationClient, LocalUserStorage localUserStorage)
    {
        _authenticationClient = authenticationClient;
        _localUserStorage = localUserStorage;
    }

    public async Task AuthenticateAsync(string username, string password, CancellationToken cancellationToken = new())
    {
        var response = await _authenticationClient.LoginAsync(new LoginInputDto
        {
            Password = password,
            Username = username
        }, cancellationToken);

        response.EnsureSuccessStatusCode();

        var user = await response.ReadPayloadAsync(cancellationToken);
        await _localUserStorage.SetUserAsync(user!, cancellationToken);
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = new())
    {
        await _localUserStorage.RemoveUserAsync(cancellationToken);
    }
}