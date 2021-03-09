using System;
using Qweree.Authentication.WebApi.Domain.Identity;

namespace Qweree.Authentication.WebApi.Infrastructure.Identity
{
    public class ClientMapper
    {
        public static Client FromDo(ClientDo document)
        {
            return new(document.Id ?? Guid.Empty,
                document.ClientId ?? string.Empty, document.ClientSecret ?? string.Empty, document.ApplicationName ?? string.Empty,
                document.CreatedAt ?? DateTime.MinValue, document.ModifiedAt ?? DateTime.MinValue,
                document.OwnerId ?? Guid.Empty);
        }

        public static ClientDo ToDo(Client client)
        {
            return new()
            {
                Id = client.Id,
                ClientId = client.ClientId,
                ClientSecret = client.ClientSecret,
                ApplicationName = client.ApplicationName,
                CreatedAt = client.CreatedAt,
                ModifiedAt = client.ModifiedAt,
                OwnerId = client.OwnerId
            };
        }
    }
}