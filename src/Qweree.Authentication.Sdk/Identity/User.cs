using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Qweree.Authentication.Sdk.Identity
{
    public class User
    {
        public User(Guid id, string username, IEnumerable<string> roles)
        {
            Id = id;
            Username = username;
            Roles = roles.ToImmutableArray();
        }

        public ImmutableArray<string> Roles { get; }
        public Guid Id { get; }
        public string Username { get; }
    }
}