using System;

namespace Qweree.Authentication.WebApi.Infrastructure.Identity
{
    public class UserDo
    {
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public string? ContactEmail { get; set; }
        public string? Password { get; set; }
        public string[]? Roles { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}