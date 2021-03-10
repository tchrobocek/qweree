using System;
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
                ModifiedAt = client.ModifiedAt
            };
        }

        public static Client FromDto(ClientDto clientDto)
        {
            return new(clientDto.Id ?? Guid.Empty, clientDto.ClientId ?? string.Empty,
                clientDto.ApplicationName ?? string.Empty, clientDto.Origin ?? string.Empty,
                UserMapper.FromDto(clientDto.Owner ?? new()), clientDto.CreatedAt ?? DateTime.MinValue,
                clientDto.ModifiedAt ?? DateTime.MinValue);
        }
    }
}