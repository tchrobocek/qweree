using System;
using System.Collections.Immutable;

namespace Qweree.Authentication.WebApi.Domain.Identity
{
    public class Client
    {
        public Client(Guid id, string clientId, string clientSecret, string applicationName, ImmutableArray<Guid> clientRoles, DateTime createdAt, DateTime modifiedAt, Guid ownerId, string origin)
        {
            Id = id;
            ClientId = clientId;
            ClientSecret = clientSecret;
            ApplicationName = applicationName;
            CreatedAt = createdAt;
            ModifiedAt = modifiedAt;
            OwnerId = ownerId;
            Origin = origin;
            ClientRoles = clientRoles;
        }

        public Guid Id { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }
        public string ApplicationName { get; }
        public string Origin { get; }
        public ImmutableArray<Guid> ClientRoles { get; }
        public DateTime CreatedAt { get; }
        public DateTime ModifiedAt { get; }
        public Guid OwnerId { get; }
    }
}