using System;
using System.Collections.Immutable;

namespace Qweree.Authentication.AdminSdk.Identity.Users.UserInvitation;

public class UserInvitationInput
{
    public UserInvitationInput(string? username, string? fullName, string? contactEmail, ImmutableArray<Guid>? roles)
    {
        Username = username;
        FullName = fullName;
        ContactEmail = contactEmail;
        Roles = roles;
    }

    public string? Username { get; }
    public string? FullName { get; }
    public string? ContactEmail { get; }
    public ImmutableArray<Guid>? Roles { get; }
}