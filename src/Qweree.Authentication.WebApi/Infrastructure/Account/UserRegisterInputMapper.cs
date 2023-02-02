using System;
using SdkUserRegisterInput = Qweree.Authentication.Sdk.Account.UserRegister.UserRegisterInput;

namespace Qweree.Authentication.WebApi.Infrastructure.Account;

public static class UserRegisterInputMapper
{
    public static Domain.Account.UserRegisterInput Map(SdkUserRegisterInput input)
    {
        return new Domain.Account.UserRegisterInput(input.UserInvitationId ?? Guid.Empty, input.Username ?? string.Empty,
            input.Fullname ?? string.Empty, input.ContactEmail ?? string.Empty, input.Password ?? string.Empty);
    }
}