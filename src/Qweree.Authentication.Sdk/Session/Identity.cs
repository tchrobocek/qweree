using System;
using Qweree.Authentication.Sdk.Identity;

namespace Qweree.Authentication.Sdk.Session;

public class Identity
{
    public IdentityClient? Client { get; set; }
    public IdentityUser? User { get; set; }
    public string? Email { get; set; }
    public string[]? Roles { get; set; }
}
public class IdentityClient
{
    public Guid? Id { get; set; }
    public string? ClientId { get; set; }
    public string? ApplicationName { get; set; }
}
public class IdentityUser
{
    public Guid? Id { get; set; }
    public string? Username { get; set; }
    public UserProperty[]? Properties { get; set; }
}