using System;
using System.Linq;
using Qweree.Authentication.WebApi.Domain.Identity;

namespace Qweree.Authentication.WebApi.Infrastructure.Identity
{
    public static class UserMapper
    {
        public static User FromDo(UserDo databaseObject)
        {
            return new User(databaseObject.Id, databaseObject.Username ?? "", databaseObject.FullName ?? "",
                databaseObject.ContactEmail ?? "", databaseObject.Password ?? "", databaseObject.Roles ?? Array.Empty<string>(),
                databaseObject.CreatedAt, databaseObject.ModifiedAt);
        }

        public static UserDo ToDo(User user)
        {
            return new UserDo
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Password = user.Password,
                ContactEmail = user.ContactEmail,
                Roles = user.Roles.ToArray(),
                CreatedAt = user.CreatedAt,
                ModifiedAt = user.ModifiedAt
            };
        }
    }
}