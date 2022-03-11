namespace Qweree.Authentication.AdminSdk.Authorization.Roles;

public class ClientEffectiveRolesCollectionDto
{
    public RoleDto[]? UserEffectiveRoles { get; set; }
    public RoleDto[]? ClientEffectiveRoles { get; set; }
}