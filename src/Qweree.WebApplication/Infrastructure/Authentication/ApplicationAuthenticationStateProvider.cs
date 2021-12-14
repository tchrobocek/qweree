using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.WebApplication.Infrastructure.Browser;

namespace Qweree.WebApplication.Infrastructure.Authentication
{
    public class ApplicationAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly LocalUserStorage _localUserStorage;

        public ApplicationAuthenticationStateProvider(LocalUserStorage localUserStorage)
        {
            _localUserStorage = localUserStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = await _localUserStorage.GetUserAsync();
            if (user == null)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(authenticationType: null)));
            }
            return new AuthenticationState(UserMapper.ToClaimsPrincipal(user));
        }
    }
}