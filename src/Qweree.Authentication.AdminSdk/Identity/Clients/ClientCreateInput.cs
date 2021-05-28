using System;
using System.Collections.Immutable;

namespace Qweree.Authentication.AdminSdk.Identity.Clients
{
    public class ClientCreateInput
    {
        public ClientCreateInput(Guid id, string clientId, string clientSecret, string applicationName, string origin, Guid ownerId, ImmutableArray<Guid> roles)
        {
            Id = id;
            ClientId = clientId;
            ClientSecret = clientSecret;
            ApplicationName = applicationName;
            Origin = origin;
            OwnerId = ownerId;
            Roles = roles;
        }

        public Guid Id { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }
        public string ApplicationName { get; }
        public Guid OwnerId { get; }
        public string Origin { get; }
        public ImmutableArray<Guid> Roles { get; }
    }
}