using System;

namespace Qweree.Authentication.AdminSdk.Identity.Clients
{
    public static class ClientCreateInputMapper
    {
        public static ClientCreateInputDto ToDto(ClientCreateInput input)
        {
            return new()
            {
                Id = input.Id,
                Origin = input.Origin,
                ApplicationName = input.ApplicationName,
                ClientId = input.ClientId,
                ClientSecret = input.ClientSecret,
                OwnerId = input.OwnerId
            };
        }
        public static ClientCreateInput FromDto(ClientCreateInputDto input)
        {
            return new(input.Id ?? Guid.Empty,
                input.ClientId ?? string.Empty,
                input.ClientSecret ?? string.Empty,
                input.ApplicationName ?? string.Empty,
                input.Origin ?? string.Empty,
                input.OwnerId ?? Guid.Empty);
        }
    }
}