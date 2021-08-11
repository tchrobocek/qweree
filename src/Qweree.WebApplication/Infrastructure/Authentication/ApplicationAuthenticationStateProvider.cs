using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace Qweree.WebApplication.Infrastructure.Authentication
{
    public class ApplicationAuthenticationStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal? _claimsPrincipal;

        public void SetClaimsPrincipal(ClaimsPrincipal? claimsPrincipal)
        {
            _claimsPrincipal = claimsPrincipal;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            ClaimsPrincipal user = _claimsPrincipal ?? new ClaimsPrincipal(CreateAnonymousIdentity());
            return Task.FromResult(new AuthenticationState(user));
        }

        private ClaimsIdentity CreateAnonymousIdentity()
        {
            return new(authenticationType: null);
        }
    }
}