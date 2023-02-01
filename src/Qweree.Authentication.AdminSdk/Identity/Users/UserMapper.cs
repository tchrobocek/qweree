using System;
using System.Collections.Immutable;
using System.Linq;
using Qweree.Authentication.AdminSdk.Authorization.Roles;

namespace Qweree.Authentication.AdminSdk.Identity.Users;

public static class UserMapper
{
    public static UserDto ToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Properties = user.Properties.Select(UserPropertyMapper.ToDto).ToArray(),
            Roles = user.Roles.Select(RoleMapper.ToDto).ToArray(),
            ContactEmail = user.ContactEmail,
            CreatedAt = user.CreatedAt,
            ModifiedAt = user.ModifiedAt
        };
    }

    public static User FromDto(UserDto user)
    {
        return new User(user.Id ?? Guid.Empty,
            user.Username ?? string.Empty,
            user.ContactEmail ?? string.Empty,
            user.Properties?.Select(UserPropertyMapper.FromDto).ToImmutableArray() ?? ImmutableArray<UserProperty>.Empty,
            user.Roles?.Select(RoleMapper.FromDto).ToImmutableArray() ?? ImmutableArray<Role>.Empty,
            user.CreatedAt ?? DateTime.MinValue,
            user.ModifiedAt ?? DateTime.MinValue);
    }
}