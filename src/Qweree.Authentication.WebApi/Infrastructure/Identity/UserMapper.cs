using System;
using System.Collections.Immutable;
using System.Linq;
using Qweree.Authentication.WebApi.Domain.Identity;

namespace Qweree.Authentication.WebApi.Infrastructure.Identity;

public static class UserMapper
{
    public static User FromDo(UserDo databaseObject)
    {
        return new User(databaseObject.Id, databaseObject.Username ?? "", databaseObject.FullName ?? "",
            databaseObject.ContactEmail ?? "", databaseObject.Password ?? "",
            databaseObject.Properties?.Select(FromDo).ToImmutableArray() ?? ImmutableArray<UserProperty>.Empty,
            databaseObject.Roles?.ToImmutableArray() ?? ImmutableArray<Guid>.Empty,
            databaseObject.CreatedAt, databaseObject.ModifiedAt);
    }
    public static UserProperty FromDo(UserPropertyDo databaseObject)
    {
        return new UserProperty(databaseObject.Key ?? string.Empty, databaseObject.Value ?? string.Empty);
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
            Properties = user.Properties.Select(ToDo).ToArray(),
            Roles = user.Roles.ToArray(),
            CreatedAt = user.CreatedAt,
            ModifiedAt = user.ModifiedAt
        };
    }

    public static UserPropertyDo ToDo(UserProperty property)
    {
        return new UserPropertyDo
        {
            Key = property.Key,
            Value = property.Value
        };
    }
}