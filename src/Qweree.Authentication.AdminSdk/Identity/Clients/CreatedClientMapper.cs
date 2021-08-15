using System;
using Qweree.Authentication.AdminSdk.Identity.Users;

namespace Qweree.Authentication.AdminSdk.Identity.Clients
{
    public static class CreatedClientMapper
    {
        public static CreatedClientDto ToDto(CreatedClient createdClient)
        {
            return new()
            {
                Id = createdClient.Id,
                Owner = UserMapper.ToDto(createdClient.Owner),
                ApplicationName = createdClient.ApplicationName,
                ClientId = createdClient.ClientId,
                ClientSecret = createdClient.ClientSecret,
                CreatedAt = createdClient.CreatedAt,
                ModifiedAt = createdClient.ModifiedAt,
                Origin = createdClient.Origin
            };
        }

        public static CreatedClient FromDto(CreatedClientDto createdClient)
        {
            return new(createdClient.Id ?? Guid.Empty,
                createdClient.ClientId ?? string.Empty,
                createdClient.ClientSecret ?? string.Empty,
                createdClient.ApplicationName ?? string.Empty,
                createdClient.Origin ?? string.Empty,
                UserMapper.FromDto(createdClient.Owner ?? new UserDto()),
                createdClient.CreatedAt ?? DateTime.MinValue,
                createdClient.ModifiedAt ?? DateTime.MinValue);
        }
    }
}