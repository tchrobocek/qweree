using System;
using System.Collections.Immutable;

namespace Qweree.Authentication.WebApi.Domain.Identity;

public class User
{
    public User(Guid id, string username, string fullName, string contactEmail, string password,
        ImmutableArray<UserProperty> properties, ImmutableArray<Guid> roles, DateTime createdAt, DateTime modifiedAt)
    {
        Id = id;
        Username = username;
        FullName = fullName;
        ContactEmail = contactEmail;
        Password = password;
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
        Properties = properties;
        Roles = roles;
    }

    public Guid Id { get; }
    public string Username { get; }
    public string FullName { get; }
    public string ContactEmail { get; }
    public string Password { get; }
    public ImmutableArray<UserProperty> Properties { get; }
    public ImmutableArray<Guid> Roles { get; }
    public DateTime CreatedAt { get; }
    public DateTime ModifiedAt { get; }
}

public class UserProperty
{
    public UserProperty(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public string Key { get; }
    public string Value { get; }
}