using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Qweree.Authentication.AdminSdk.Identity.Users
{
    public class UserCreateInput
    {
        public UserCreateInput(Guid id, string username, string contactEmail, string fullName, string password,
            IEnumerable<Guid> roles)
        {
            Id = id;
            Username = username;
            ContactEmail = contactEmail;
            FullName = fullName;
            Password = password;
            Roles = roles.ToImmutableArray();
        }

        public Guid Id { get; }
        public string Username { get; }
        public string ContactEmail { get; }
        public string FullName { get; }
        public string Password { get; }
        public ImmutableArray<Guid> Roles { get; }
    }
}