using System;

namespace Qweree.Authentication.Sdk.Identity
{
    public class UserDto
    {
        public Guid? Id { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Username { get; set; }
        public string[]? Roles { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DateTime? CreatedAt { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DateTime? ModifiedAt { get; set; }
    }
}