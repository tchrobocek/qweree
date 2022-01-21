using System;

namespace Qweree.Authentication.AdminSdk.Authorization.Roles;

public class RoleDto
{
    public Guid? Id { get; set; }
    public string? Key { get; set; }
    public string? Label { get; set; }
    public string? Description { get; set; }
}