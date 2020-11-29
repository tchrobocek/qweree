using System.Linq;

namespace Qweree.Authentication.Sdk.Identity
{
    public class UserCreateInputMapper
    {
        public static UserCreateInputDto ToDto(UserCreateInput userCreateInput)
        {
            return new UserCreateInputDto
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
            return new UserCreateInput(userCreateInput.Username ?? "", userCreateInput.ContactEmail ?? "",
                userCreateInput.FullName ?? "", userCreateInput.Password ?? "",
                userCreateInput.Roles ?? System.Array.Empty<string>());
        }
    }
}