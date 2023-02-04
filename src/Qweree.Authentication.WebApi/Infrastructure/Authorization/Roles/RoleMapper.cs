using System;
using System.Linq;
using System.Collections.Immutable;
using Role = Qweree.Authentication.WebApi.Domain.Authorization.Roles.Role;

namespace Qweree.Authentication.WebApi.Infrastructure.Authorization.Roles;

public static class RoleMapper
{
    public static RoleDo ToRoleDo(Role role)
    {
        return new RoleDo
        {
            Description = role.Description,
            Id = role.Id,
            Items = role.Items.ToArray(),
            Label = role.Label,
            CreatedAt = role.CreatedAt,
            IsGroup = role.IsGroup,
            ModifiedAt = role.ModifiedAt,
            Key = role.Key
        };
    }

    public static Role ToRole(RoleDo roleDo)
    {
        return new Role(
            roleDo.Id ?? Guid.Empty,
            roleDo.Key ?? string.Empty,
            roleDo.Label ?? string.Empty,
            roleDo.Description ?? string.Empty,
            roleDo.Items?.ToImmutableArray() ?? ImmutableArray<Guid>.Empty,
            roleDo.IsGroup ?? false,
            roleDo.CreatedAt ?? DateTime.MinValue,
            roleDo.ModifiedAt ?? DateTime.MinValue
        );
    }
}