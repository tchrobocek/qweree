using System;
using System.Linq;
using System.Collections.Immutable;
using UserRole = Qweree.Authentication.WebApi.Domain.Authorization.Roles.UserRole;
using ClientRole = Qweree.Authentication.WebApi.Domain.Authorization.Roles.ClientRole;

namespace Qweree.Authentication.WebApi.Infrastructure.Authorization.Roles;

public static class RoleMapper
{
    public static UserRoleDo ToUserRoleDo(UserRole role)
    {
        return new UserRoleDo
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

    public static UserRole ToUserRole(UserRoleDo roleDo)
    {
        return new UserRole(
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

    public static ClientRoleDo ToClientRoleDo(ClientRole role)
    {
        return new ClientRoleDo
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

    public static ClientRole ToClientRole(ClientRoleDo roleDo)
    {
        return new ClientRole(
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