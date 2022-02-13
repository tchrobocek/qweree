using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;

namespace Qweree.Authentication.Sdk.Session;

public static class IdentityMapper
{
    public static ClaimsPrincipal ToClaimsPrincipal(Identity identity)
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

    public static Identity ToIdentity(ClaimsPrincipal claimsPrincipal)
    {
        return ToIdentity(claimsPrincipal.Claims);
    }

    public static Identity ToIdentity(IEnumerable<Claim> claims)
    {
        claims = claims.ToArray();

        var email = claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "anonymous";
        var roles = claims.Where(c => c.Type is ClaimTypes.Role or "role")
            .Select(c => c.Value)
            .Distinct()
            .ToArray();

        var client = ToIdentityClient(claims);

        if (!roles.Contains("CLIENT"))
        {
            var user = ToIdentityUser(claims);
            return new Identity(client, user, email, roles.ToImmutableArray());
        }

        return new Identity(client, email, roles.ToImmutableArray());
    }

    public static IdentityUser ToIdentityUser(ClaimsPrincipal claimsPrincipal)
    {
        return ToIdentityUser(claimsPrincipal.Claims);
    }

    private static IdentityUser ToIdentityUser(IEnumerable<Claim> claims)
    {
        claims = claims.ToArray();

        var id = claims.FirstOrDefault(c => c.Type == "user.id")?.Value ?? Guid.Empty.ToString();
        var username = claims.FirstOrDefault(c => c.Type == "user.username")?.Value ?? "anonymous";
        var fullName = claims.FirstOrDefault(c => c.Type == "user.full_name")?.Value ?? "anonymous";
        return new IdentityUser(Guid.Parse(id), username, fullName);
    }

    public static IdentityClient ToIdentityClient(ClaimsPrincipal claimsPrincipal)
    {
        return ToIdentityClient(claimsPrincipal.Claims);
    }

    public static IdentityClient ToIdentityClient(IEnumerable<Claim> claims)
    {
        claims = claims.ToArray();

        var id = claims.FirstOrDefault(c => c.Type == "client.id")?.Value ?? Guid.Empty.ToString();
        var clientId = claims.FirstOrDefault(c => c.Type == "client.client_id")?.Value ?? "anonymous";
        var appName = claims.FirstOrDefault(c => c.Type == "client.application_name")?.Value ?? "anonymous";
        return new IdentityClient(Guid.Parse(id), clientId, appName);
    }
}