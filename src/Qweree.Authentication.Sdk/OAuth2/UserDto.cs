using System;

namespace Qweree.Authentication.Sdk.OAuth2;

public class UserDto
{
    public Guid? Id { get; set; }
    public string? Username { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string[]? Roles { get; set; }
}