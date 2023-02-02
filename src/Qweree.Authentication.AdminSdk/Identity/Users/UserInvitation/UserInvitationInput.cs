using System;

namespace Qweree.Authentication.AdminSdk.Identity.Users.UserInvitation;

public class UserInvitationInput
{
    public string? Username { get; set; }
    public string? FullName { get; set; }
    public string? ContactEmail { get; set; }
    public Guid[]? Roles { get; set; }
}