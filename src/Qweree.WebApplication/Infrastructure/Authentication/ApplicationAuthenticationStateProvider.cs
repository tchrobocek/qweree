using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Qweree.WebApplication.Infrastructure.Browser;

namespace Qweree.WebApplication.Infrastructure.Authentication
{
    public class ApplicationAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly LocalStorage _localStorage;

        public ApplicationAuthenticationStateProvider(LocalStorage localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var accessToken = await _localStorage.GetItemAsync("access_token");

            ClaimsIdentity user;

            if (accessToken == null)
            {
                user = new ClaimsIdentity(authenticationType: null);
            }
            else
            {
                var claims = TokenDecoder.ReadClaims(accessToken);
                user = new ClaimsIdentity(claims, "oauth2");
            }

            return new AuthenticationState(new ClaimsPrincipal(user));
        }
    }
}