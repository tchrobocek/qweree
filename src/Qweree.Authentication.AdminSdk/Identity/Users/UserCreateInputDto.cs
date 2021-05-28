using System;

namespace Qweree.Authentication.AdminSdk.Identity.Users
{
    public class UserCreateInputDto
    {
        public Guid? Id { get; set; }
        public string? Username { get; set; }
        public string? ContactEmail { get; set; }
        public string? FullName { get; set; }
        public string? Password { get; set; }
        public Guid[]? Roles { get; set; }
    }
}