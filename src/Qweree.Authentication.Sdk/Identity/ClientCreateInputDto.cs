using System;

namespace Qweree.Authentication.Sdk.Identity
{
    public class ClientCreateInputDto
    {
        public Guid? Id { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? ApplicationName { get; set; }
        public string? Origin { get; set; }
        public Guid? OwnerId { get; set; }
    }
}