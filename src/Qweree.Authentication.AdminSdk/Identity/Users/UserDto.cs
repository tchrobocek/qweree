using System;

namespace Qweree.Authentication.AdminSdk.Identity.Users
{
    public class UserDto
    {
        public Guid? Id { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public string? ContactEmail { get; set; }
        public string[] Roles { get; set; } = new string[0];
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}