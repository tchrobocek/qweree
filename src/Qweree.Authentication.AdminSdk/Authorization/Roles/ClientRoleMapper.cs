using System;
using System.Collections.Immutable;
using System.Linq;

namespace Qweree.Authentication.AdminSdk.Authorization.Roles
{
    public static class ClientRoleMapper
    {
        public static ClientRoleCreateInput FromDto(ClientRoleCreateInputDto dto)
        {
            return new(dto.Id ?? Guid.Empty, dto.Key ?? string.Empty, dto.Label ?? string.Empty,
                dto.Description ?? string.Empty, dto.IsGroup ?? false, dto.Items?.ToImmutableArray() ??
                                                                       ImmutableArray<Guid>.Empty);
        }

        public static ClientRoleModifyInput FromDto(Guid id, ClientRoleModifyInputDto dto)
        {
            return new(id, dto.Label, dto.Description, dto.IsGroup, dto.Items?.ToImmutableArray());
        }

        public static ClientRole FromDto(ClientRoleDto dto)
        {
            return new(dto.Id ?? Guid.Empty, dto.Key ?? string.Empty, dto.Label ?? string.Empty,
                dto.Description ?? string.Empty,
                dto.Items?.Select(FromDto).ToImmutableArray() ?? ImmutableArray<ClientRole>.Empty, dto.IsGroup ?? false,
                dto.CreatedAt ?? DateTime.MinValue, dto.ModifiedAt ?? DateTime.MinValue, dto.EffectiveRoles?.Select(RoleMapper.FromDto)
                    .ToImmutableArray() ?? ImmutableArray<Role>.Empty);
        }

        public static ClientRoleCreateInputDto ToDto(ClientRoleCreateInput input)
        {
            return new()
            {
                Id = input.Id,
                Description = input.Description,
                Items = input.Items.ToArray(),
                Key = input.Key,
                IsGroup = input.IsGroup,
                Label = input.Label
            };
        }

        public static ClientRoleDto ToDto(ClientRole input)
        {
            return new()
            {
                Id = input.Id,
                Description = input.Description,
                Items = input.Items.Select(ToDto).ToArray(),
                Key = input.Key,
                IsGroup = input.IsGroup,
                Label = input.Label,
                CreatedAt = input.CreatedAt,
                EffectiveRoles = input.EffectiveRoles.Select(RoleMapper.ToDto).ToArray(),
                ModifiedAt = input.ModifiedAt
            };
        }

        public static ClientRoleModifyInputDto ToDto(ClientRoleModifyInput input)
        {
            return new()
            {
                Description = input.Description,
                Items = input.Items?.ToArray(),
                IsGroup = input.IsGroup,
                Label = input.Label
            };
        }
    }
}