using System;
using System.Collections.Immutable;

namespace Qweree.Authentication.AdminSdk.Identity.Users.UserRegister;

public class UserInvitationDescriptor
{
    public UserInvitationDescriptor(Guid id, string? username, string? fullName, string? contactEmail,
        ImmutableArray<Guid>? roles, DateTime expiresAt, DateTime createdAt, DateTime modifiedAt)
    {
        Id = id;
        Username = username;
        FullName = fullName;
        ContactEmail = contactEmail;
        Roles = roles;
        ExpiresAt = expiresAt;
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }

    public Guid Id { get; }
    public string? Username { get; }
    public string? FullName { get; }
    public string? ContactEmail { get; }
    public ImmutableArray<Guid>? Roles { get; }
    public DateTime ExpiresAt { get; }
    public DateTime CreatedAt { get; }
    public DateTime ModifiedAt { get; }
}