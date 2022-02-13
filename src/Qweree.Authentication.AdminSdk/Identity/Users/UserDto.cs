using System;
using Qweree.Authentication.AdminSdk.Authorization.Roles;
using Qweree.Authentication.Sdk.Users;

namespace Qweree.Authentication.AdminSdk.Identity.Users;

public class UserDto
{
    public Guid? Id { get; set; }
    public string? Username { get; set; }
    public string? FullName { get; set; }
    public string? ContactEmail { get; set; }
    public UserPropertyDto[]? Properties { get; set; }
    public RoleDto[]? Roles { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
