using System;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using ClaimsPrincipalMapper = Qweree.Authentication.Sdk.Session.ClaimsPrincipalMapper;
using Identity = Qweree.Authentication.Sdk.Session.Identity;
using IdentityClient = Qweree.Authentication.Sdk.Session.IdentityClient;
using IdentityUser = Qweree.Authentication.Sdk.Session.IdentityUser;

namespace Qweree.Authentication.Sdk.OAuth2;

public class IdentityMapper
{
    public static Identity Map(IdentityDto identityDto)
    {
        var client = Map(identityDto.Client!);

        if (identityDto.User != null)
        {
            return new Identity(client, Map(identityDto.User), identityDto.Email ?? string.Empty,
                identityDto.Roles?.ToImmutableArray() ?? ImmutableArray<string>.Empty);
        }

        return new Identity(client, identityDto.Email ?? string.Empty,
            identityDto.Roles?.ToImmutableArray() ?? ImmutableArray<string>.Empty);
    }

    public static IdentityClient Map(IdentityClientDto identityDto)
    {
        return new IdentityClient(identityDto.Id ?? Guid.Empty, identityDto.ClientId ?? string.Empty,
            identityDto.ApplicationName ?? string.Empty);
    }

    public static IdentityUser Map(IdentityUserDto identityDto)
    {
        return new IdentityUser(identityDto.Id ?? Guid.Empty, identityDto.Username ?? string.Empty,
            identityDto.FullName ?? string.Empty);
    }

    public static IdentityDto Map(Identity identity)
    {
        var dto = new IdentityDto
        {
            Client = Map(identity.Client),
            Email = identity.Email,
            Roles = identity.Roles.ToArray()
        };

        if (identity.User != null)
            dto.User = Map(identity.User);

        return dto;
    }

    public static IdentityClientDto Map(IdentityClient identity)
    {
        return new IdentityClientDto
        {
            Id = identity.Id,
            ApplicationName = identity.ApplicationName,
            ClientId = identity.ClientId
        };
    }

    public static IdentityUserDto Map(IdentityUser identity)
    {
        return new IdentityUserDto
        {
            Id = identity.Id,
            Username = identity.Username,
            FullName = identity.FullName
        };
    }

    public static ClaimsPrincipal ToClaimsPrincipal(IdentityDto identityDto)
    {
        return ClaimsPrincipalMapper.CreateClaimsPrincipal(Map(identityDto));
    }

    public static IdentityDto FromClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
    {
        return Map(ClaimsPrincipalMapper.CreateIdentity(claimsPrincipal));
    }
}