using System;

namespace Qweree.Authentication.Sdk.OAuth2;

public class IdentityDto
{
    public IdentityClientDto? Client { get; set; }
    public IdentityUserDto? User { get; set; }
    public string? Email { get; set; }
    public string[]? Roles { get; set; }
}
public class IdentityClientDto
{
    public Guid? Id { get; set; }
    public string? ClientId { get; set; }
    public string? ApplicationName { get; set; }
}
public class IdentityUserDto
{
    public Guid? Id { get; set; }
    public string? Username { get; set; }
    public string? FullName { get; set; }
}