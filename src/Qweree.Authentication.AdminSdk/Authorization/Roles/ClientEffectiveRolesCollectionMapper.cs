using System.Collections.Immutable;
using System.Linq;

namespace Qweree.Authentication.AdminSdk.Authorization.Roles;

public static class ClientEffectiveRolesCollectionMapper
{
    public static ClientEffectiveRolesCollectionDto ToDto(ClientEffectiveRolesCollection collection)
    {
        return new ClientEffectiveRolesCollectionDto
        {
            ClientEffectiveRoles = collection.ClientEffectiveRoles.Select(RoleMapper.ToDto).ToArray(),
            UserEffectiveRoles = collection.UserEffectiveRoles.Select(RoleMapper.ToDto).ToArray(),
        };
    }
    public static ClientEffectiveRolesCollection FromDto(ClientEffectiveRolesCollectionDto dto)
    {
        return new ClientEffectiveRolesCollection(
            dto.UserEffectiveRoles?.Select(RoleMapper.FromDto).ToImmutableArray() ?? ImmutableArray<Role>.Empty,
            dto.ClientEffectiveRoles?.Select(RoleMapper.FromDto).ToImmutableArray() ?? ImmutableArray<Role>.Empty
        );
    }
}