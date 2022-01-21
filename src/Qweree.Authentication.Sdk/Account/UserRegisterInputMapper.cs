using System;

namespace Qweree.Authentication.Sdk.Account;

public static class UserRegisterInputMapper
{
    public static UserRegisterInput FromDto(UserRegisterInputDto dto)
    {
        return new UserRegisterInput(dto.UserInvitationId ?? Guid.Empty, dto.Username ?? string.Empty,
            dto.Fullname ?? string.Empty, dto.ContactEmail ?? string.Empty, dto.Password ?? string.Empty);
    }

    public static UserRegisterInputDto ToDto(UserRegisterInput input)
    {
        return new UserRegisterInputDto
        {
            Fullname = input.Fullname,
            Password = input.Password,
            Username = input.Username,
            ContactEmail = input.ContactEmail,
            UserInvitationId = input.UserInvitationId
        };
    }

}