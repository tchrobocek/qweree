using System;
using System.Linq;
using System.Security.Claims;
using Qweree.Authentication.Sdk.Identity;

namespace Qweree.AspNet.Session
{
    public class ClaimsPrincipalStorage : ISessionStorage
    {
        private static User CreateUser(ClaimsPrincipal claimsPrincipal)
        {
            var id = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "id")?.Value ?? "";
            var username = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "username")?.Value ?? "anonymous";
            var roles = claimsPrincipal.Claims.Where(c => c.Type == "username").Select(c => c.Value);
            return new User(Guid.Parse(id), username, roles);
        }

        public ClaimsPrincipalStorage(ClaimsPrincipal claimsPrincipal)
        {
            ClaimsPrincipal = claimsPrincipal;
            CurrentUser = CreateUser(claimsPrincipal);
        }

        public ClaimsPrincipal ClaimsPrincipal { get; }
        public User CurrentUser { get; }
    }
}