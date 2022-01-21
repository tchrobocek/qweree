using System;

namespace Qweree.Authentication.AdminSdk.Authorization.Roles;

public class ClientRoleModifyInputDto
{
    public string? Label { get; set; }
    public string? Description { get; set; }
    public bool? IsGroup { get; set; }
    public Guid[]? Items { get; set; }
}