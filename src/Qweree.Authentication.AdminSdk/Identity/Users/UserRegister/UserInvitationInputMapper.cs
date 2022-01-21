using System;
using System.Collections.Immutable;
using System.Linq;

namespace Qweree.Authentication.AdminSdk.Identity.Users.UserRegister;

public class UserInvitationInputMapper
{
    public static UserInvitationInputDto ToDto(UserInvitationInput userInvitationInput)
    {
        return new UserInvitationInputDto
        {
            Roles = (userInvitationInput.Roles ?? ImmutableArray<Guid>.Empty).ToArray(),
            Username = userInvitationInput.Username,
            ContactEmail = userInvitationInput.ContactEmail,
            FullName = userInvitationInput.FullName
        };
    }

    public static UserInvitationInput FromDto(UserInvitationInputDto userInvitationInputDto)
    {
        return new UserInvitationInput(userInvitationInputDto.Username,
            userInvitationInputDto.FullName, userInvitationInputDto.ContactEmail,
            userInvitationInputDto.Roles?.ToImmutableArray());
    }
}