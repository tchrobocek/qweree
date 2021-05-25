using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Qweree.Authentication.WebApi.Domain.Identity
{
    public class User
    {
        public User(Guid id, string username, string fullName, string contactEmail, string password,
            IEnumerable<Guid> roles, DateTime createdAt, DateTime modifiedAt)
        {
            Id = id;
            Username = username;
            FullName = fullName;
            ContactEmail = contactEmail;
            Password = password;
            CreatedAt = createdAt;
            ModifiedAt = modifiedAt;
            Roles = roles.ToImmutableArray();
        }

        public Guid Id { get; }
        public string Username { get; }
        public string FullName { get; }
        public string ContactEmail { get; }
        public string Password { get; }
        public ImmutableArray<Guid> Roles { get; }
        public DateTime CreatedAt { get; }
        public DateTime ModifiedAt { get; }
    }
}