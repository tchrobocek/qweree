using System.Collections.Immutable;

namespace Qweree.Session;

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
    public IdentityUser(Guid id, string username, string fullName)
    {
        Id = id;
        Username = username;
        FullName = fullName;
    }

    public Guid Id { get; }
    public string Username { get; }
    public string FullName { get; }
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