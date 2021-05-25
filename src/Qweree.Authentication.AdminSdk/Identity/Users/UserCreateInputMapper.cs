using System;
using System.Linq;

namespace Qweree.Authentication.AdminSdk.Identity.Users
{
    public class UserCreateInputMapper
    {
        public static UserCreateInputDto ToDto(UserCreateInput userCreateInput)
        {
            return new()
            {
                Password = userCreateInput.Password,
                Roles = userCreateInput.Roles.ToArray(),
                Username = userCreateInput.Username,
                ContactEmail = userCreateInput.ContactEmail,
                FullName = userCreateInput.FullName
            };
        }

        public static UserCreateInput FromDto(UserCreateInputDto userCreateInput)
        {
            return new(userCreateInput.Username ?? "", userCreateInput.ContactEmail ?? "",
                userCreateInput.FullName ?? "", userCreateInput.Password ?? "",
                userCreateInput.Roles ?? Array.Empty<string>());
        }
    }
}