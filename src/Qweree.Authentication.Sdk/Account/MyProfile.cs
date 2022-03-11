using System;
using System.Collections.Immutable;
using Qweree.Authentication.Sdk.Users;

namespace Qweree.Authentication.Sdk.Account;

public class MyProfile
{
    public MyProfile(Guid id, string username, string contactEmail, ImmutableArray<UserProperty> properties)
    {
        Id = id;
        Username = username;
        ContactEmail = contactEmail;
        Properties = properties;
    }

    public Guid Id { get; }
    public string Username { get; }
    public string ContactEmail { get; }
    public ImmutableArray<UserProperty> Properties { get; }
}