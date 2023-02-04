using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Qweree.Authentication.Sdk.Identity;

namespace Qweree.Authentication.Sdk.Session;

public static class IdentityMapper
{
    public static ClaimsPrincipal ToClaimsPrincipal(Identity identity)
    {
        var claims = new List<Claim>
        {
            new("client.id", identity.Client?.Id.ToString() ?? Guid.Empty.ToString()),
            new("client.client_id", identity.Client?.ClientId ?? string.Empty),
            new("client.application_name", identity.Client?.ApplicationName ?? string.Empty),
            new("email", identity.Email ?? string.Empty)
        };

        claims.AddRange(identity.Roles?.Select(r => new Claim("role", r)) ?? Array.Empty<Claim>());
        claims.AddRange(identity.Roles?.Select(r => new Claim(ClaimTypes.Role, r)) ?? Array.Empty<Claim>());

        if (identity.User is not null)
        {
            claims.Add(new Claim("user.id", identity.User.Id.ToString() ?? Guid.Empty.ToString()));
            claims.Add(new Claim("user.username", identity.User.Username ?? string.Empty));
            claims.AddRange(identity.User.Properties?.Select(prop => new Claim($"user.property.{prop.Key}", prop.Value ?? string.Empty)) ?? Array.Empty<Claim>());
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

        IdentityUser? user = null;
        if (claims.Any(c => c.Type == "user.id"))
            user = ToIdentityUser(claims);

        return new Identity
        {
            Client = client,
            Email = email,
            Roles = roles,
            User = user
        };
    }

    public static IdentityClient ToIdentityClient(IEnumerable<Claim> claims)
    {
        claims = claims.ToArray();

        var id = claims.FirstOrDefault(c => c.Type == "client.id")?.Value ?? Guid.Empty.ToString();
        var clientId = claims.FirstOrDefault(c => c.Type == "client.client_id")?.Value ?? "anonymous";
        var appName = claims.FirstOrDefault(c => c.Type == "client.application_name")?.Value ?? "anonymous";
        return new IdentityClient
        {
            Id = Guid.Parse(id),
            ClientId = clientId,
            ApplicationName = appName
        };
    }

    private static IdentityUser ToIdentityUser(IEnumerable<Claim> claims)
    {
        claims = claims.ToArray();

        var id = claims.FirstOrDefault(c => c.Type == "user.id")?.Value ?? Guid.Empty.ToString();
        var username = claims.FirstOrDefault(c => c.Type == "user.username")?.Value ?? "anonymous";

        var properties = claims.Where(c => c.Type.StartsWith("user.property."))
            .Select(c => new AuthUserProperty
            {
                Key = c.Type["user.property.".Length..],
                Value = c.Value
            })
            .ToArray();

        return new IdentityUser
        {
            Id = Guid.Parse(id),
            Username = username,
            Properties = properties
        };
    }
}