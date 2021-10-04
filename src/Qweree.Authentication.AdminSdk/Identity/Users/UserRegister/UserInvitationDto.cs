using System;

namespace Qweree.Authentication.AdminSdk.Identity.Users.UserRegister
{
    public class UserInvitationDto
    {
        public Guid? Id { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public string? ContactEmail { get; set; }
        public Guid[]? Roles { get; set; }
    }
}