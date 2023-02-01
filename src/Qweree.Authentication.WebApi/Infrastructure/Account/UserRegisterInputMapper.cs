using System;
using Qweree.Authentication.Sdk.Account;
using SdkUserRegisterInput = Qweree.Authentication.Sdk.Account.UserRegisterInput;

namespace Qweree.Authentication.WebApi.Infrastructure.Account;

public static class UserRegisterInputMapper
{
    public static Domain.Account.UserRegisterInput Map(UserRegisterInput input)
    {
        return new Domain.Account.UserRegisterInput(input.UserInvitationId ?? Guid.Empty, input.Username ?? string.Empty,
            input.Fullname ?? string.Empty, input.ContactEmail ?? string.Empty, input.Password ?? string.Empty);
    }
}