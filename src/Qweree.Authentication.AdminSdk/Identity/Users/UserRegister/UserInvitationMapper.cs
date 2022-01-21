using System;
using System.Collections.Immutable;
using System.Linq;

namespace Qweree.Authentication.AdminSdk.Identity.Users.UserRegister;

public static class UserInvitationMapper
{
    public static UserInvitationDto ToDto(UserInvitation userInvitation)
    {
        return new UserInvitationDto
        {
            Id = userInvitation.Id,
            Roles = (userInvitation.Roles ?? ImmutableArray<Guid>.Empty).ToArray(),
            Username = userInvitation.Username,
            ContactEmail = userInvitation.ContactEmail,
            FullName = userInvitation.FullName,
            CreatedAt = userInvitation.CreatedAt,
            ExpiresAt = userInvitation.ExpiresAt,
            ModifiedAt = userInvitation.ModifiedAt,
        };
    }

    public static UserInvitation FromDto(UserInvitationDto userInvitationDto)
    {
        return new UserInvitation(userInvitationDto.Id ?? Guid.Empty, userInvitationDto.Username,
            userInvitationDto.FullName, userInvitationDto.ContactEmail,
            userInvitationDto.Roles?.ToImmutableArray(), userInvitationDto.ExpiresAt ?? DateTime.MinValue, userInvitationDto.CreatedAt ?? DateTime.MinValue,
            userInvitationDto.ModifiedAt ?? DateTime.MinValue);
    }
}