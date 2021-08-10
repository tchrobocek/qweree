using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace Qweree.WebApplication.Infrastructure.Authentication
{
    public class ClaimsPrincipalStorage
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public ClaimsPrincipalStorage(AuthenticationStateProvider authenticationStateProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<User?> GetUser()
        {
            var authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();

            if (!authenticationState.User.Identity?.IsAuthenticated ?? false)
            {
                return null;
            }

            return CreateUser(authenticationState.User);
        }

        private static User CreateUser(ClaimsPrincipal claimsPrincipal)
        {
            var id = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "userId")?.Value ?? Guid.Empty.ToString();
            var username = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "username")?.Value ?? "anonymous";
            var fullName = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "full_name")?.Value ?? "anonymous";
            var email = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "anonymous";
            var roles = claimsPrincipal.Claims.Where(c => c.Type == "roles").Select(c => c.Value);
            return new User(Guid.Parse(id), username, fullName, email, roles);
        }
    }
}