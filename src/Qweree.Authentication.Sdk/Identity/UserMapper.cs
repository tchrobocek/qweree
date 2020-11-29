using System;
using System.Linq;

namespace Qweree.Authentication.Sdk.Identity
{
    public static class UserMapper
    {
        public static UserDto ToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Roles = user.Roles.ToArray()
            };
        }

        public static User FromDto(UserDto user)
        {
            return new User(user.Id ?? Guid.Empty, user.Username ?? "", user.Roles ?? Array.Empty<string>());
        }
    }
}