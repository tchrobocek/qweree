using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Qweree.WebApplication.Infrastructure.Authentication;

public class User
{
    public User(Guid id, string username, string fullName, string email, IEnumerable<string> roles)
    {
        Id = id;
        Username = username;
        FullName = fullName;
        Email = email;
        Roles = roles.ToImmutableArray();
    }

    public Guid Id { get; }
    public string Username { get; }
    public string FullName { get; }
    public string Email { get; }
    public ImmutableArray<string> Roles { get; }
}