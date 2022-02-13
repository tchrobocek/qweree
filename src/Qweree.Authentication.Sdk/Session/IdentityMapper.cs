using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;

namespace Qweree.Authentication.Sdk.Session;

public static class IdentityMapper
{
    public static ClaimsPrincipal FromDto(Identity identity)
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

    public static Identity ToDto(IdentityDto identityDto)
    {
        var client = ToDto(identityDto.Client!);

        if (identityDto.User != null)
        {
            return new Identity(client, ToDto(identityDto.User), identityDto.Email ?? string.Empty,
                identityDto.Roles?.ToImmutableArray() ?? ImmutableArray<string>.Empty);
        }

        return new Identity(client, identityDto.Email ?? string.Empty,
            identityDto.Roles?.ToImmutableArray() ?? ImmutableArray<string>.Empty);
    }

    public static IdentityClient ToDto(IdentityClientDto identityDto)
    {
        return new IdentityClient(identityDto.Id ?? Guid.Empty, identityDto.ClientId ?? string.Empty,
            identityDto.ApplicationName ?? string.Empty);
    }

    public static IdentityUser ToDto(IdentityUserDto identityDto)
    {
        return new IdentityUser(identityDto.Id ?? Guid.Empty, identityDto.Username ?? string.Empty,
            identityDto.FullName ?? string.Empty);
    }

    public static IdentityDto ToDto(Identity identity)
    {
        var dto = new IdentityDto
        {
            Client = ToDto(identity.Client),
            Email = identity.Email,
            Roles = identity.Roles.ToArray()
        };

        if (identity.User != null)
            dto.User = ToDto(identity.User);

        return dto;
    }

    public static IdentityClientDto ToDto(IdentityClient identity)
    {
        return new IdentityClientDto
        {
            Id = identity.Id,
            ApplicationName = identity.ApplicationName,
            ClientId = identity.ClientId
        };
    }

    public static IdentityUserDto ToDto(IdentityUser identity)
    {
        return new IdentityUserDto
        {
            Id = identity.Id,
            Username = identity.Username,
            FullName = identity.FullName
        };
    }

    public static ClaimsPrincipal FromDto(IdentityDto identityDto)
    {
        return FromDto(ToDto(identityDto));
    }

    public static IdentityDto ToDto(ClaimsPrincipal claimsPrincipal)
    {
        return ToDto(ToIdentity(claimsPrincipal));
    }
}