using System;
using System.Collections.Immutable;
using Qweree.Authentication.AdminSdk.Authorization.Roles;

namespace Qweree.Authentication.AdminSdk.Identity.Users
{
    public class User
    {
        public User(Guid id, string username, string fullName, string contactEmail, ImmutableArray<Role> roles, DateTime createdAt, DateTime modifiedAt)
        {
            Id = id;
            Username = username;
            FullName = fullName;
            ContactEmail = contactEmail;
            Roles = roles;
            CreatedAt = createdAt;
            ModifiedAt = modifiedAt;
        }

        public Guid Id { get; }
        public string Username { get; }
        public string FullName { get; }
        public string ContactEmail { get; }
        public ImmutableArray<Role> Roles { get; }
        public DateTime CreatedAt { get; }
        public DateTime ModifiedAt { get; }
    }
}