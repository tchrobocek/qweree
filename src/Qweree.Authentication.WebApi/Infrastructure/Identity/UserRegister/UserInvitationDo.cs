using System;

namespace Qweree.Authentication.WebApi.Infrastructure.Identity.UserRegister
{
    public class UserInvitationDo
    {
        public Guid? Id { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public string? ContactEmail { get; set; }
        public Guid[]? Roles { get; set; }
    }
}