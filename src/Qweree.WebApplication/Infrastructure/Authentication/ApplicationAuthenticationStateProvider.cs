using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Qweree.Authentication.Sdk.Session;
using Qweree.WebApplication.Infrastructure.Browser;

namespace Qweree.WebApplication.Infrastructure.Authentication;

public class ApplicationAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly LocalUserStorage _localUserStorage;

    public ApplicationAuthenticationStateProvider(LocalUserStorage localUserStorage)
    {
        _localUserStorage = localUserStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = await _localUserStorage.GetIdentityAsync();
        if (identity == null)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(authenticationType: null)));
        }
        return new AuthenticationState(IdentityMapper.ToClaimsPrincipal(identity));
    }
}