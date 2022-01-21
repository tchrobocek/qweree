using System;
using System.Collections.Immutable;
using System.Linq;
using Qweree.Authentication.AdminSdk.Identity.Users.UserRegister;

namespace Qweree.Authentication.WebApi.Infrastructure.Identity.UserRegister;

public static class UserInvitationMapper
{
    public static UserInvitationDo ToDo(UserInvitation userInvitation)
    {
        return new UserInvitationDo
        {
            Id = userInvitation.Id,
            Username = userInvitation.Username,
            Roles = (userInvitation.Roles ?? ImmutableArray<Guid>.Empty).ToArray(),
            ContactEmail = userInvitation.ContactEmail,
            FullName = userInvitation.FullName,
            CreatedAt = userInvitation.CreatedAt,
            ModifiedAt = userInvitation.ModifiedAt,
            ExpiresAt = userInvitation.ExpiresAt
        };
    }

    public static UserInvitation FromDo(UserInvitationDo userInvitationDo)
    {
        return new UserInvitation(userInvitationDo.Id ?? Guid.Empty, userInvitationDo.Username,
            userInvitationDo.FullName, userInvitationDo.ContactEmail, userInvitationDo.Roles?.ToImmutableArray() ?? ImmutableArray<Guid>.Empty,
            userInvitationDo.ExpiresAt ?? DateTime.MinValue, userInvitationDo.CreatedAt ?? DateTime.MinValue, userInvitationDo.ModifiedAt ?? DateTime.MinValue);
    }
}