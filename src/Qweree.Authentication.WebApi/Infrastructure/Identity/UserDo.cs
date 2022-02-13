using System;

namespace Qweree.Authentication.WebApi.Infrastructure.Identity;

public class UserDo
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public string? ContactEmail { get; set; }
    public string? Password { get; set; }
    public UserPropertyDo[]? Properties { get; set; }
    public Guid[]? Roles { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}

public class UserPropertyDo
{
    public string? Key { get; set; }
    public string? Value { get; set; }
}