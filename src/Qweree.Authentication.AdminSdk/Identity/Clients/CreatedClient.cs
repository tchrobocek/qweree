using System;
using System.Collections.Immutable;
using Qweree.Authentication.AdminSdk.Authorization.Roles;
using Qweree.Authentication.AdminSdk.Identity.Users;

namespace Qweree.Authentication.AdminSdk.Identity.Clients
{
    public class CreatedClient
    {
        public CreatedClient(Guid id, string clientId, string clientSecret, string applicationName, string origin, User owner, DateTime createdAt,
            DateTime modifiedAt, ImmutableArray<Role> clientRoles)
        {
            Id = id;
            ClientId = clientId;
            ClientSecret = clientSecret;
            ApplicationName = applicationName;
            Origin = origin;
            CreatedAt = createdAt;
            ModifiedAt = modifiedAt;
            ClientRoles = clientRoles;
            Owner = owner;
        }

        public Guid Id { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }
        public string ApplicationName { get; }
        public string Origin { get; }
        public DateTime CreatedAt { get; }
        public DateTime ModifiedAt { get; }
        public User Owner { get; }
        public ImmutableArray<Role> ClientRoles { get; }
    }
}