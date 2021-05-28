using System;

namespace Qweree.Authentication.AdminSdk.Authorization.Roles
{
    public static class RoleMapper
    {
        public static RoleDto ToDto(Role role)
        {
            return new()
            {
                Id = role.Id,
                Key = role.Key,
                Label = role.Label,
                Description = role.Description
            };
        }

        public static Role FromDto(RoleDto role)
        {
            return new(role.Id ?? Guid.Empty, role.Key ?? string.Empty, role.Label ?? string.Empty, role.Description ??
                string.Empty);
        }

        public static Role FromUserRole(UserRole userRole)
        {
            return new(userRole.Id, userRole.Key, userRole.Label, userRole.Description);
        }

        public static Role FromClientRole(ClientRole clientRole)
        {
            return new(clientRole.Id, clientRole.Key, clientRole.Label, clientRole.Description);
        }
    }
}