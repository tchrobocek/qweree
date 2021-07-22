using System;
using System.Collections.Immutable;
using System.Linq;

namespace Qweree.Authentication.AdminSdk.Identity.Users
{
    public class UserCreateInputMapper
    {
        public static UserCreateInputDto ToDto(UserCreateInput userCreateInput)
        {
            return new()
            {
                Id = userCreateInput.Id,
                Password = userCreateInput.Password,
                Roles = userCreateInput.Roles.ToArray(),
                Username = userCreateInput.Username,
                ContactEmail = userCreateInput.ContactEmail,
                FullName = userCreateInput.FullName
            };
        }

        public static UserCreateInput FromDto(UserCreateInputDto userCreateInput)
        {
            return new(userCreateInput.Id ?? Guid.Empty, userCreateInput.Username ?? "", userCreateInput.ContactEmail ?? "",
                userCreateInput.FullName ?? "", userCreateInput.Password ?? "",
                userCreateInput.Roles?.ToImmutableArray() ?? ImmutableArray<Guid>.Empty);
        }
    }
}