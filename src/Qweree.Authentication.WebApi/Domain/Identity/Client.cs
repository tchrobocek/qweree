using System;
using System.Collections.Immutable;

namespace Qweree.Authentication.WebApi.Domain.Identity
{
    public class Client
    {
        public Client(Guid id, string clientId, string clientSecret, DateTime createdAt, DateTime modifiedAt, Guid ownerId)
        {
            Id = id;
            ClientId = clientId;
            ClientSecret = clientSecret;
            CreatedAt = createdAt;
            ModifiedAt = modifiedAt;
            OwnerId = ownerId;
        }

        public Guid Id { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }
        public DateTime CreatedAt { get; }
        public DateTime ModifiedAt { get; }
        public Guid OwnerId { get; }
    }
}