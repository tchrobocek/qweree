using System;
using Qweree.Authentication.AdminSdk.Identity.Users;

namespace Qweree.Authentication.AdminSdk.Identity.Clients
{
    public class CreatedClient
    {
        public CreatedClient(Guid id, string clientId, string clientSecret, string applicationName, User owner, DateTime createdAt,
            DateTime modifiedAt)
        {
            Id = id;
            ClientId = clientId;
            ClientSecret = clientSecret;
            ApplicationName = applicationName;
            CreatedAt = createdAt;
            ModifiedAt = modifiedAt;
            Owner = owner;
        }

        public Guid Id { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }
        public string ApplicationName { get; }
        public DateTime CreatedAt { get; }
        public DateTime ModifiedAt { get; }
        public User Owner { get; }
    }
}