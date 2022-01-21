using System;

namespace Qweree.Authentication.AdminSdk.Authorization.Roles;

public class UserRoleCreateInputDto
{
    public Guid? Id { get; set; }
    public string? Key { get; set; }
    public string? Label { get; set; }
    public string? Description { get; set; }
    public bool? IsGroup { get; set; }
    public Guid[]? Items { get; set; }
}