using System;
using System.Collections.Immutable;
using System.Linq;

namespace Qweree.Authentication.AdminSdk.Authorization.Roles
{
    public static class RoleMapper
    {
        public static CreateUserRoleInput FromDto(CreateUserRoleInputDto dto)
        {
            return new(dto.Id ?? Guid.Empty, dto.Key ?? string.Empty, dto.Label ?? string.Empty,
                dto.Description ?? string.Empty, dto.IsGroup ?? false, dto.Items?.ToImmutableArray() ??
                                                                       ImmutableArray<Guid>.Empty);
        }

        public static ModifyUserRoleInput FromDto(ModifyUserRoleInputDto dto)
        {
            return new(dto.Id ?? Guid.Empty, dto.Label, dto.Description, dto.IsGroup, dto.Items?.ToImmutableArray());
        }

        public static UserRole FromDto(UserRoleDto dto)
        {
            return new(dto.Id ?? Guid.Empty, dto.Key ?? string.Empty, dto.Label ?? string.Empty,
                dto.Description ?? string.Empty,
                dto.Items?.Select(FromDto).ToImmutableArray() ?? ImmutableArray<UserRole>.Empty, dto.IsGroup ?? false,
                dto.CreatedAt ?? DateTime.MinValue, dto.ModifiedAt ?? DateTime.MinValue, dto.EffectiveRoles
                    ?.ToImmutableArray() ?? ImmutableArray<string>.Empty);
        }

        public static CreateUserRoleInputDto ToDto(CreateUserRoleInput input)
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
                Label = input.Label
            };
        }

        public static ModifyUserRoleInputDto ToDto(ModifyUserRoleInput input)
        {
            return new()
            {
                Id = input.Id,
                Description = input.Description,
                Items = input.Items?.ToArray(),
                IsGroup = input.IsGroup,
                Label = input.Label
            };
        }

        public static CreateClientRoleInput FromDto(CreateClientRoleInputDto dto)
        {
            return new(dto.Id ?? Guid.Empty, dto.Key ?? string.Empty, dto.Label ?? string.Empty,
                dto.Description ?? string.Empty, dto.IsGroup ?? false, dto.Items?.ToImmutableArray() ??
                                                                       ImmutableArray<Guid>.Empty);
        }

        public static ModifyClientRoleInput FromDto(ModifyClientRoleInputDto dto)
        {
            return new(dto.Id ?? Guid.Empty, dto.Label, dto.Description, dto.IsGroup, dto.Items?.ToImmutableArray());
        }

        public static ClientRole FromDto(ClientRoleDto dto)
        {
            return new(dto.Id ?? Guid.Empty, dto.Key ?? string.Empty, dto.Label ?? string.Empty,
                dto.Description ?? string.Empty,
                dto.Items?.Select(FromDto).ToImmutableArray() ?? ImmutableArray<ClientRole>.Empty, dto.IsGroup ?? false,
                dto.CreatedAt ?? DateTime.MinValue, dto.ModifiedAt ?? DateTime.MinValue, dto.EffectiveRoles
                    ?.ToImmutableArray() ?? ImmutableArray<string>.Empty);
        }

        public static CreateClientRoleInputDto ToDto(CreateClientRoleInput input)
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
                Label = input.Label
            };
        }

        public static ModifyClientRoleInputDto ToDto(ModifyClientRoleInput input)
        {
            return new()
            {
                Id = input.Id,
                Description = input.Description,
                Items = input.Items?.ToArray(),
                IsGroup = input.IsGroup,
                Label = input.Label
            };
        }
    }
}