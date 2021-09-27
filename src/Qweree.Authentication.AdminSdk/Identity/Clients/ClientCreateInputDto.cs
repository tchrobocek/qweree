using System;

namespace Qweree.Authentication.AdminSdk.Identity.Clients
{
    public class ClientCreateInputDto
    {
        public Guid? Id { get; set; }
        public string? ClientId { get; set; }
        public string? ApplicationName { get; set; }
        public string? Origin { get; set; }
        public Guid? OwnerId { get; set; }
        public Guid[]? ClientRoles { get; set; }
    }
}