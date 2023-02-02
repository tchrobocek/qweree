using System;

namespace Qweree.Authentication.Sdk.Account.UserRegister;

public class UserInvitation
{
    public Guid? Id { get; set; }
    public string? Username { get; set; }
    public string? FullName { get; set; }
    public string? ContactEmail { get; set; }
    public DateTime? ExpiresAt { get; set; }
}