using System;
using System.Collections.Immutable;
using System.Linq;

namespace Qweree.Authentication.AdminSdk.Authorization.Roles
{
    public static class UserRoleMapper
    {
        public static UserRoleCreateInput FromDto(UserRoleCreateInputDto dto)
        {
            return new(dto.Id ?? Guid.Empty, dto.Key ?? string.Empty, dto.Label ?? string.Empty,
                dto.Description ?? string.Empty, dto.IsGroup ?? false, dto.Items?.ToImmutableArray() ??
                                                                       ImmutableArray<Guid>.Empty);
        }

        public static UserRoleModifyInput FromDto(Guid id, UserRoleModifyInputDto dto)
        {
            return new(id, dto.Label, dto.Description, dto.IsGroup, dto.Items?.ToImmutableArray());
        }

        public static UserRole FromDto(UserRoleDto dto)
        {
            return new(dto.Id ?? Guid.Empty, dto.Key ?? string.Empty, dto.Label ?? string.Empty,
                dto.Description ?? string.Empty,
                dto.Items?.Select(FromDto).ToImmutableArray() ?? ImmutableArray<UserRole>.Empty, dto.IsGroup ?? false,
                dto.CreatedAt ?? DateTime.MinValue, dto.ModifiedAt ?? DateTime.MinValue, dto.EffectiveRoles
                    ?.ToImmutableArray() ?? ImmutableArray<string>.Empty);
        }

        public static UserRoleCreateInputDto ToDto(UserRoleCreateInput input)
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

        public static UserRoleDto ToDto(UserRole input)
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
                EffectiveRoles = input.EffectiveRoles.ToArray(),
                ModifiedAt = input.ModifiedAt
            };
        }

        public static UserRoleModifyInputDto ToDto(UserRoleModifyInput input)
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