using System;
using System.Collections.Immutable;
using System.Linq;
using Qweree.Authentication.AdminSdk.Authorization.Roles;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;

namespace Qweree.Authentication.WebApi.Infrastructure.Authorization.Roles;

public static class RoleMapper
{
    public static UserRoleDo ToDo(UserRole role)
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

    public static UserRole FromDo(UserRoleDo roleDo)
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

    public static ClientRoleDo ToDo(ClientRole role)
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

    public static ClientRole FromDo(ClientRoleDo roleDo)
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

    public static UserRoleCreateInput FromDto(UserRoleCreateInputDto dto)
    {
        return new UserRoleCreateInput(dto.Id ?? Guid.Empty, dto.Key ?? string.Empty, dto.Label ?? string.Empty,
            dto.Description ?? string.Empty, dto.IsGroup ?? false, dto.Items?.ToImmutableArray() ??
                                                                   ImmutableArray<Guid>.Empty);
    }

    public static UserRoleModifyInput FromDto(Guid id, UserRoleModifyInputDto dto)
    {
        return new UserRoleModifyInput(id, dto.Label, dto.Description, dto.IsGroup, dto.Items?.ToImmutableArray());
    }

    public static ClientRoleCreateInput FromDto(ClientRoleCreateInputDto dto)
    {
        return new ClientRoleCreateInput(dto.Id ?? Guid.Empty, dto.Key ?? string.Empty, dto.Label ?? string.Empty,
            dto.Description ?? string.Empty, dto.IsGroup ?? false, dto.Items?.ToImmutableArray() ??
                                                                   ImmutableArray<Guid>.Empty);
    }

    public static ClientRoleModifyInput FromDto(Guid id, ClientRoleModifyInputDto dto)
    {
        return new ClientRoleModifyInput(id, dto.Label, dto.Description, dto.IsGroup, dto.Items?.ToImmutableArray());
    }
}