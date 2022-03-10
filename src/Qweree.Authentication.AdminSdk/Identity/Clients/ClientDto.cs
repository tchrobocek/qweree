using System;
using Qweree.Authentication.AdminSdk.Authorization.Roles;
using Qweree.Authentication.AdminSdk.Identity.Users;

namespace Qweree.Authentication.AdminSdk.Identity.Clients;

public class ClientDto
{
    public Guid? Id { get; set; }
    public string? ClientId { get; set; }
    public string? ApplicationName { get; set; }
    public string? Origin { get; set; }
    public UserDto? Owner { get; set; }
    public RoleDto[]? ClientRoles { get; set; }
    public RoleDto[]? EffectiveClientRoles { get; set; }
    public RoleDto[]? UserRoles { get; set; }
    public RoleDto[]? EffectiveUserRoles { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}