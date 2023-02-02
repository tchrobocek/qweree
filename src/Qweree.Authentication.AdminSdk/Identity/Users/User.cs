using System;
using Qweree.Authentication.AdminSdk.Authorization.Roles;

namespace Qweree.Authentication.AdminSdk.Identity.Users;

public class User
{
    public Guid? Id { get; set; }
    public string? Username { get; set; }
    public string? ContactEmail { get; set; }
    public UserProperty[]? Properties { get; set; }
    public Role[]? Roles { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}

public class UserProperty
{
    public string? Key { get; set; }
    public string? Value { get; set; }
}