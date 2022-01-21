using System;

namespace Qweree.Authentication.Sdk.Account;

public class UserInvitation
{
    public UserInvitation(Guid id, string? username, string? fullName, string? contactEmail,
        DateTime expiresAt)
    {
        Id = id;
        Username = username;
        FullName = fullName;
        ContactEmail = contactEmail;
        ExpiresAt = expiresAt;
    }

    public Guid Id { get; }
    public string? Username { get; }
    public string? FullName { get; }
    public string? ContactEmail { get; }
    public DateTime ExpiresAt { get; }
}