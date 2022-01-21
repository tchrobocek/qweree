using System;

namespace Qweree.Authentication.Sdk.Account;

public class UserRegisterInput
{
    public UserRegisterInput(Guid userInvitationId, string username, string fullname, string contactEmail, string password)
    {
        UserInvitationId = userInvitationId;
        Username = username;
        Fullname = fullname;
        ContactEmail = contactEmail;
        Password = password;
    }

    public Guid UserInvitationId { get; }
    public string Username { get; }
    public string Fullname { get; }
    public string ContactEmail { get; }
    public string Password { get; }
}