using System;
using System.Collections.Immutable;
using System.Linq;
using Qweree.Authentication.WebApi.Domain.Identity;

namespace Qweree.Authentication.WebApi.Infrastructure.Identity;

public static class ClientMapper
{
    public static Client FromDo(ClientDo document)
    {
        return new Client(document.Id ?? Guid.Empty, document.ClientId ?? string.Empty, document.ClientSecret ?? string.Empty,
            document.ApplicationName ?? string.Empty, document.ClientRoles?.ToImmutableArray() ?? ImmutableArray<Guid>.Empty, document.UserRoles?.ToImmutableArray() ?? ImmutableArray<Guid>.Empty, document.CreatedAt ?? DateTime.MinValue,
            document.ModifiedAt ?? DateTime.MinValue, document.OwnerId ?? Guid.Empty, document.Origin ?? String.Empty);
    }

    public static ClientDo ToDo(Client client)
    {
        return new ClientDo
        {
            Id = client.Id,
            ClientId = client.ClientId,
            ClientSecret = client.ClientSecret,
            ApplicationName = client.ApplicationName,
            CreatedAt = client.CreatedAt,
            ModifiedAt = client.ModifiedAt,
            OwnerId = client.OwnerId,
            Origin = client.Origin,
            ClientRoles = client.ClientRoles.ToArray(),
            UserRoles = client.UserRoles.ToArray()
        };
    }
}