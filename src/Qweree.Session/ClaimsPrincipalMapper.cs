using System.Collections.Immutable;
using System.Security.Claims;

namespace Qweree.Session;

public class ClaimsPrincipalMapper
{
    public static ClaimsPrincipal CreateClaimsPrincipal(Identity identity)
    {
        var claims = new List<Claim>
        {
            new("client.id", identity.Client.Id.ToString()),
            new("client.client_id", identity.Client.ClientId),
            new("client.application_name", identity.Client.ApplicationName),
            new("email", identity.Email)
        };

        claims.AddRange(identity.Roles.Select(r => new Claim("role", r)));

        if (identity.User != null)
        {
            claims.Add(new Claim("user.id", identity.User.Id.ToString()));
            claims.Add(new Claim("user.username", identity.User.Username));
            claims.Add(new Claim("user.full_name", identity.User.FullName));
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "qweree"));
    }

    public static Identity CreateIdentity(ClaimsPrincipal claimsPrincipal)
    {
        var email = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "anonymous";
        var roles = claimsPrincipal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value)
            .ToArray();

        var client = CreateClient(claimsPrincipal);

        if (!roles.Contains("CLIENT"))
        {
            var user = CreateUser(claimsPrincipal);
            return new Identity(client, user, email, roles.ToImmutableArray());
        }

        return new Identity(client, email, roles.ToImmutableArray());
    }

    private static IdentityUser CreateUser(ClaimsPrincipal claimsPrincipal)
    {
        var id = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "user.id")?.Value ?? Guid.Empty.ToString();
        var username = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "user.username")?.Value ?? "anonymous";
        var fullName = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "user.full_name")?.Value ?? "anonymous";
        return new IdentityUser(Guid.Parse(id), username, fullName);
    }

    private static IdentityClient CreateClient(ClaimsPrincipal claimsPrincipal)
    {
        var id = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "client.id")?.Value ?? Guid.Empty.ToString();
        var clientId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "client.client_id")?.Value ?? "anonymous";
        var appName = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "client.application_name")?.Value ?? "anonymous";
        return new IdentityClient(Guid.Parse(id), clientId, appName);
    }
}