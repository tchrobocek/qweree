using System;
using System.Linq;
using System.Security.Claims;

namespace Qweree.AspNet.Session;

public class ClaimsPrincipalStorage : ISessionStorage
{
    public ClaimsPrincipalStorage(ClaimsPrincipal claimsPrincipal)
    {
        ClaimsPrincipal = claimsPrincipal;
        CurrentUser = CreateUser(claimsPrincipal);
        CurrentClient = CreateClient(claimsPrincipal);
    }

    public ClaimsPrincipal ClaimsPrincipal { get; }
    public User? CurrentUser { get; }
    public Client CurrentClient { get; }
    public Guid Id => CurrentUser?.Id ?? CurrentClient.ClientId;

    private static User? CreateUser(ClaimsPrincipal claimsPrincipal)
    {
        var roles = claimsPrincipal.Claims.Where(c => c.Type == "roles").Select(c => c.Value)
            .ToArray();

        if (roles.Contains("CLIENT"))
            return null;

        var id = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value ?? Guid.Empty.ToString();
        var username = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "username")?.Value ?? "anonymous";
        var fullName = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "full_name")?.Value ?? "anonymous";
        var email = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "anonymous";
        return new User(Guid.Parse(id), username, fullName, email, roles);
    }

    private static Client CreateClient(ClaimsPrincipal claimsPrincipal)
    {
        var id = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "userId")?.Value ?? Guid.Empty.ToString();
        return new Client(Guid.Parse(id));
    }
}