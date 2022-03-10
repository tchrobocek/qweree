using System;
using System.Collections.Immutable;
using Qweree.Authentication.AdminSdk.Authorization.Roles;
using Qweree.Authentication.Sdk.Users;

namespace Qweree.Authentication.AdminSdk.Identity.Users;

public class User
{
    public User(Guid id, string username, string contactEmail, ImmutableArray<UserProperty> properties, ImmutableArray<Role> roles, ImmutableArray<Role> effectiveRoles,
        DateTime createdAt, DateTime modifiedAt)
    {
        Id = id;
        Username = username;
        ContactEmail = contactEmail;
        Properties = properties;
        Roles = roles;
        EffectiveRoles = effectiveRoles;
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }

    public Guid Id { get; }
    public string Username { get; }
    public string ContactEmail { get; }
    public ImmutableArray<Role> Roles { get; }
    public ImmutableArray<Role> EffectiveRoles { get; }
    public ImmutableArray<UserProperty> Properties { get; }
    public DateTime CreatedAt { get; }
    public DateTime ModifiedAt { get; }
}
