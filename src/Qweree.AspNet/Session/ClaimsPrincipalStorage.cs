using System.Linq;
using System.Security.Claims;

namespace Qweree.AspNet.Session
{
    public class ClaimsPrincipalStorage : ISessionStorage
    {
        private static Session CreateSession(ClaimsPrincipal claimsPrincipal)
        {
            var username = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "username")?.Value ?? "anonymous";
            var roles = claimsPrincipal.Claims.Where(c => c.Type == "username").Select(c => c.Value);
            return new Session(username, roles);
        }

        public ClaimsPrincipalStorage(ClaimsPrincipal claimsPrincipal)
        {
            ClaimsPrincipal = claimsPrincipal;
            CurrentSession = CreateSession(claimsPrincipal);
        }

        public ClaimsPrincipal ClaimsPrincipal { get; }
        public Session CurrentSession { get; }
    }
}