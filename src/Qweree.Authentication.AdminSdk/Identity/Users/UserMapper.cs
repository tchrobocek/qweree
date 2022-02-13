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
            Properties = user.Properties.Select(ToDto).ToArray(),
            Roles = user.Roles.Select(RoleMapper.ToDto).ToArray(),
            ContactEmail = user.ContactEmail,
            CreatedAt = user.CreatedAt,
            FullName = user.FullName,
            ModifiedAt = user.ModifiedAt
        };
    }
    public static UserPropertyDto ToDto(UserProperty property)
    {
        return new UserPropertyDto
        {
            Key = property.Key,
            Value = property.Value
        };
    }

    public static User FromDto(UserDto user)
    {
        return new User(user.Id ?? Guid.Empty,
            user.Username ?? string.Empty,
            user.FullName ?? string.Empty,
            user.ContactEmail ?? string.Empty,
            user.Properties?.Select(FromDto).ToImmutableArray() ?? ImmutableArray<UserProperty>.Empty,
            user.Roles?.Select(RoleMapper.FromDto).ToImmutableArray() ?? ImmutableArray<Role>.Empty,
            user.CreatedAt ?? DateTime.MinValue,
            user.ModifiedAt ?? DateTime.MinValue);
    }

    public static UserProperty FromDto(UserPropertyDto property)
    {
        return new UserProperty(property.Key ?? string.Empty, property.Value ?? string.Empty);
    }
}