using System;
using System.Linq;
using System.Security.Claims;

namespace Qweree.AspNet.Session
{
    public class ClaimsPrincipalStorage : ISessionStorage
    {
        private static User CreateUser(ClaimsPrincipal claimsPrincipal)
        {
            var id = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "userId")?.Value ?? "";
            var username = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "username")?.Value ?? "anonymous";
            var fullName = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "full_name")?.Value ?? "anonymous";
            var email = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "anonymous";
            var roles = claimsPrincipal.Claims.Where(c => c.Type == "roles").Select(c => c.Value);
            return new User(Guid.Parse(id), username, fullName, email, roles);
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