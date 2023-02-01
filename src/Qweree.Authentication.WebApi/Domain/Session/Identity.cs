using System;
using System.Collections.Immutable;
using UserProperty = Qweree.Authentication.WebApi.Domain.Identity.UserProperty;

namespace Qweree.Authentication.WebApi.Domain.Session;

public class Identity
{
    public Identity(IdentityClient client, string email, ImmutableArray<string> roles)
    {
        Client = client;
        Email = email;
        Roles = roles;
    }

    public Identity(IdentityClient client, IdentityUser user, string email, ImmutableArray<string> roles)
        :this(client, email, roles)
    {
        User = user;
    }

    public IdentityClient Client { get; }
    public IdentityUser? User { get; }
    public string Email { get; }
    public ImmutableArray<string> Roles { get; }
}

public class IdentityUser
{
    public IdentityUser(Guid id, string username, ImmutableArray<UserProperty> properties)
    {
        Id = id;
        Username = username;
        Properties = properties;
    }

    public Guid Id { get; }
    public string Username { get; }
    public ImmutableArray<UserProperty> Properties { get; }
}

public class IdentityClient
{
    public IdentityClient(Guid id, string clientId, string applicationName)
    {
        Id = id;
        ClientId = clientId;
        ApplicationName = applicationName;
    }

    public Guid Id { get; }
    public string ClientId { get; }
    public string ApplicationName { get; }
}