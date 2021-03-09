using System;

namespace Qweree.Authentication.WebApi.Domain.Identity
{
    public class Client
    {
        public Client(Guid id, string clientId, string clientSecret, string applicationName, DateTime createdAt, DateTime modifiedAt, Guid ownerId)
        {
            Id = id;
            ClientId = clientId;
            ClientSecret = clientSecret;
            ApplicationName = applicationName;
            CreatedAt = createdAt;
            ModifiedAt = modifiedAt;
            OwnerId = ownerId;
        }

        public Guid Id { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }
        public string ApplicationName { get; }
        public DateTime CreatedAt { get; }
        public DateTime ModifiedAt { get; }
        public Guid OwnerId { get; }
    }
}