using System;
using Qweree.Authentication.AdminSdk.Identity.Users;

namespace Qweree.Authentication.AdminSdk.Identity.Clients
{
    public class ClientDto
    {
        public Guid? Id { get; set; }
        public string? ClientId { get; set; }
        public string? ApplicationName { get; set; }
        public string? Origin { get; set; }
        public UserDto? Owner { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}