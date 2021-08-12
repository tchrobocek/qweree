using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Qweree.WebApplication.Infrastructure.Browser;

namespace Qweree.WebApplication.Infrastructure.Authentication
{
    public class ApplicationAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly LocalTokenStorage _localTokenStorage;

        public ApplicationAuthenticationStateProvider(LocalTokenStorage localTokenStorage)
        {
            _localTokenStorage = localTokenStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var accessToken = await _localTokenStorage.GetAccessTokenAsync();

            ClaimsIdentity user;

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                user = new ClaimsIdentity(authenticationType: null);
            }
            else
            {
                var claims = TokenDecoder.ReadClaims(accessToken)
                    .ToList();
                claims.AddRange(claims.Where(c => c.Type == "role").Select(c => new Claim(ClaimTypes.Role, c.Value)).ToArray());
                user = new ClaimsIdentity(claims, "oauth2");
            }

            return new AuthenticationState(new ClaimsPrincipal(user));
        }
    }
}