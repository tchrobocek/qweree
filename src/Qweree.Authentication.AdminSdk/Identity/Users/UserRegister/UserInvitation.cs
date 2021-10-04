using System;
using System.Collections.Immutable;

namespace Qweree.Authentication.AdminSdk.Identity.Users.UserRegister
{
    public class UserInvitation
    {
        public UserInvitation(Guid id, string? username, string? fullName, string? contactEmail,
            ImmutableArray<Guid>? roles)
        {
            Id = id;
            Username = username;
            FullName = fullName;
            ContactEmail = contactEmail;
            Roles = roles;
        }

        public Guid Id { get; }
        public string? Username { get; }
        public string? FullName { get; }
        public string? ContactEmail { get; }
        public ImmutableArray<Guid>? Roles { get; }
    }
}