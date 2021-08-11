using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace Qweree.WebApplication.Infrastructure.Authentication
{
    public class ApplicationAuthenticationStateProvider : AuthenticationStateProvider
    {
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = new ClaimsPrincipal(CreateAnonymousIdentity());
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
                new Claim("userId", Guid.NewGuid().ToString()),
                new Claim("username", "user"),
                new Claim("full_name", "user userowitch"),
                new Claim("email", "user@email.com"),
            }, "auth-type");
        }
    }
}