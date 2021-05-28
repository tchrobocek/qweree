using System;
using System.Collections.Immutable;
using System.Linq;
using Qweree.Authentication.AdminSdk.Authorization.Roles;
using Qweree.Authentication.AdminSdk.Identity.Users;

namespace Qweree.Authentication.AdminSdk.Identity.Clients
{
    public static class ClientMapper
    {
        public static ClientDto ToDto(Client client)
        {
            return new()
            {
                Id = client.Id,
                Origin = client.Origin,
                Owner = UserMapper.ToDto(client.Owner),
                ApplicationName = client.ApplicationName,
                ClientId = client.ClientId,
                CreatedAt = client.CreatedAt,
                ModifiedAt = client.ModifiedAt,
                Roles = client.Roles.Select(RoleMapper.ToDto).ToArray()
            };
        }

        public static Client FromDto(ClientDto clientDto)
        {
            return new(clientDto.Id ?? Guid.Empty, clientDto.ClientId ?? string.Empty,
                clientDto.ApplicationName ?? string.Empty, clientDto.Origin ?? string.Empty,
                UserMapper.FromDto(clientDto.Owner ?? new()), clientDto.Roles?.Select(RoleMapper.FromDto).ToImmutableArray() ?? ImmutableArray<Role>.Empty,
                clientDto.CreatedAt ?? DateTime.MinValue,
                clientDto.ModifiedAt ?? DateTime.MinValue);
        }
    }
}