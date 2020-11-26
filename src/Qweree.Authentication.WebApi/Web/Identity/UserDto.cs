using System;

namespace Qweree.Authentication.WebApi.Web.Identity
{
    public class UserDto
    {
        public Guid? Id { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
    }
}