using System;
using System.Collections.Immutable;
using System.Linq;

namespace Qweree.Authentication.AdminSdk.Identity.Clients;

public static class ClientCreateInputMapper
{
    public static ClientCreateInputDto ToDto(ClientCreateInput input)
    {
        return new ClientCreateInputDto
        {
            Id = input.Id,
            Origin = input.Origin,
            ApplicationName = input.ApplicationName,
            ClientId = input.ClientId,
            OwnerId = input.OwnerId,
            ClientRoles = input.ClientRoles.ToArray(),
            UserRoles = input.UserRoles.ToArray()
        };
    }
    public static ClientCreateInput FromDto(ClientCreateInputDto input)
    {
        return new ClientCreateInput(input.Id ?? Guid.Empty,
            input.ClientId ?? string.Empty,
            input.ApplicationName ?? string.Empty,
            input.Origin ?? string.Empty,
            input.OwnerId ?? Guid.Empty,
            input.ClientRoles?.ToImmutableArray() ?? ImmutableArray<Guid>.Empty,
            input.UserRoles?.ToImmutableArray() ?? ImmutableArray<Guid>.Empty);
    }
}