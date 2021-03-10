using System;

namespace Qweree.Authentication.WebApi.Domain.Identity
{
    public class ClientCreateInput
    {
        public ClientCreateInput(Guid id, string clientId, string clientSecret, string applicationName, string origin, Guid ownerId)
        {
            Id = id;
            ClientId = clientId;
            ClientSecret = clientSecret;
            ApplicationName = applicationName;
            Origin = origin;
            OwnerId = ownerId;
        }

        public Guid Id { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }
        public string ApplicationName { get; }
        public Guid OwnerId { get; }
        public string Origin { get; }
    }
}