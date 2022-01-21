using System;

namespace Qweree.Authentication.Sdk.Account;

public static class UserInvitationMapper
{
    public static UserInvitationDto ToDto(UserInvitation userInvitation)
    {
        return new UserInvitationDto
        {
            Id = userInvitation.Id,
            Username = userInvitation.Username,
            ContactEmail = userInvitation.ContactEmail,
            FullName = userInvitation.FullName,
            ExpiresAt = userInvitation.ExpiresAt,
        };
    }

    public static UserInvitation FromDto(UserInvitationDto userInvitationDto)
    {
        return new UserInvitation(userInvitationDto.Id ?? Guid.Empty, userInvitationDto.Username,
            userInvitationDto.FullName, userInvitationDto.ContactEmail,  userInvitationDto.ExpiresAt ?? DateTime.MinValue);
    }
}