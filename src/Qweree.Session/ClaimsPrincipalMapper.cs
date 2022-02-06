using System.Collections.Immutable;
using System.Security.Claims;

namespace Qweree.Session;

public static class ClaimsPrincipalMapper
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
        claims.AddRange(identity.Roles.Select(r => new Claim(ClaimTypes.Role, r)));

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
        return CreateIdentity(claimsPrincipal.Claims);
    }

    public static Identity CreateIdentity(IEnumerable<Claim> claims)
    {
        claims = claims.ToArray();

        var email = claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "anonymous";
        var roles = claims.Where(c => c.Type is ClaimTypes.Role or "role")
            .Select(c => c.Value)
            .Distinct()
            .ToArray();

        var client = CreateClient(claims);

        if (!roles.Contains("CLIENT"))
        {
            var user = CreateUser(claims);
            return new Identity(client, user, email, roles.ToImmutableArray());
        }

        return new Identity(client, email, roles.ToImmutableArray());
    }

    public static IdentityUser CreateUser(ClaimsPrincipal claimsPrincipal)
    {
        return CreateUser(claimsPrincipal.Claims);
    }

    private static IdentityUser CreateUser(IEnumerable<Claim> claims)
    {
        claims = claims.ToArray();

        var id = claims.FirstOrDefault(c => c.Type == "user.id")?.Value ?? Guid.Empty.ToString();
        var username = claims.FirstOrDefault(c => c.Type == "user.username")?.Value ?? "anonymous";
        var fullName = claims.FirstOrDefault(c => c.Type == "user.full_name")?.Value ?? "anonymous";
        return new IdentityUser(Guid.Parse(id), username, fullName);
    }

    public static IdentityClient CreateClient(ClaimsPrincipal claimsPrincipal)
    {
        return CreateClient(claimsPrincipal.Claims);
    }

    public static IdentityClient CreateClient(IEnumerable<Claim> claims)
    {
        claims = claims.ToArray();

        var id = claims.FirstOrDefault(c => c.Type == "client.id")?.Value ?? Guid.Empty.ToString();
        var clientId = claims.FirstOrDefault(c => c.Type == "client.client_id")?.Value ?? "anonymous";
        var appName = claims.FirstOrDefault(c => c.Type == "client.application_name")?.Value ?? "anonymous";
        return new IdentityClient(Guid.Parse(id), clientId, appName);
    }
}