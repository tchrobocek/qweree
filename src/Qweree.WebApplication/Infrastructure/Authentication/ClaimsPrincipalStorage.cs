using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Qweree.Authentication.Sdk.OAuth2;

namespace Qweree.WebApplication.Infrastructure.Authentication;

public class ClaimsPrincipalStorage
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public ClaimsPrincipalStorage(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<IdentityDto?> GetIdentityAsync(CancellationToken cancellationToken = new())
    {
        var authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();

        if (!(authenticationState.User.Identity?.IsAuthenticated ?? false))
        {
            return null;
        }

        return IdentityMapper.FromClaimsPrincipal(authenticationState.User);
    }

    public async Task<ClaimsPrincipal> GetClaimsPrincipalAsync(CancellationToken cancellationToken = new())
    {
        var authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return authenticationState.User;
    }
}