using System;
using System.Collections.Immutable;
using System.Linq;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;

namespace Qweree.Authentication.WebApi.Infrastructure.Authorization.Roles
{
    public static class UserRoleMapper
    {
        public static UserRoleDo ToDo(UserRole role)
        {
            return new()
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
            return new(
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
    public static class ClientRoleMapper
    {
        public static ClientRoleDo ToDo(ClientRole role)
        {
            return new()
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
            return new(
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
}