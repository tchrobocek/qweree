using System;

namespace Qweree.Authentication.Sdk.Account;

public class UserRegisterInputDto
{
    public Guid? UserInvitationId { get; set; }
    public string? Username { get; set; }
    public string? Fullname { get; set; }
    public string? ContactEmail { get; set; }
    public string? Password { get; set; }
}