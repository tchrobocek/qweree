using System;
using System.Collections.Immutable;
using System.Linq;

namespace Qweree.Authentication.AdminSdk.Identity.Users
{
    public static class UserMapper
    {
        public static UserDto ToDto(User user)
        {
            return new()
            {
                Id = user.Id,
                Username = user.Username,
                Roles = user.Roles.ToArray(),
                ContactEmail = user.ContactEmail,
                CreatedAt = user.CreatedAt,
                FullName = user.FullName,
                ModifiedAt = user.ModifiedAt
            };
        }

        public static User FromDto(UserDto user)
        {
            return new(user.Id ?? Guid.Empty,
                user.Username ?? string.Empty,
                user.FullName ?? string.Empty,
                user.ContactEmail ?? string.Empty,
                user.Roles?.ToImmutableArray() ?? ImmutableArray<string>.Empty,
                user.CreatedAt ?? DateTime.MinValue,
                user.ModifiedAt ?? DateTime.MinValue);
        }
    }
}