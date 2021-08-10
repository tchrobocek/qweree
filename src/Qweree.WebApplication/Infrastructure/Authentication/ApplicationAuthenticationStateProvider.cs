using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace Qweree.WebApplication.Infrastructure.Authentication
{
    public class ApplicationAuthenticationStateProvider : AuthenticationStateProvider
    {
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = new ClaimsPrincipal(CreateAuthenticatedIdentity());
            return Task.FromResult(new AuthenticationState(user));
        }

        private ClaimsIdentity CreateAnonymousIdentity()
        {
            return new(authenticationType: null);
        }

        private ClaimsIdentity CreateAuthenticatedIdentity()
        {
            return new(new[]
            {
                new Claim(ClaimTypes.Name, "admin"),
            }, "auth-type");
        }
    }
}