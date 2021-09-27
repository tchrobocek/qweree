using System;
using System.Collections.Immutable;
using Qweree.Authentication.AdminSdk.Authorization.Roles;
using Qweree.Authentication.AdminSdk.Identity.Users;

namespace Qweree.Authentication.AdminSdk.Identity.Clients
{
    public class Client
    {
        public Client(Guid id, string clientId, string applicationName, string origin, User owner, ImmutableArray<Role> clientRoles, DateTime createdAt, DateTime modifiedAt)
        {
            Id = id;
            ClientId = clientId;
            ApplicationName = applicationName;
            Origin = origin;
            Owner = owner;
            ClientRoles = clientRoles;
            CreatedAt = createdAt;
            ModifiedAt = modifiedAt;
        }

        public Guid Id { get; }
        public string ClientId { get; }
        public string ApplicationName { get; }
        public string Origin { get; }
        public User Owner { get; }
        public ImmutableArray<Role> ClientRoles { get; }
        public DateTime CreatedAt { get; }
        public DateTime ModifiedAt { get; }
    }
}