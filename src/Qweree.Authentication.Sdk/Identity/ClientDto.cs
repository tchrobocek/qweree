using System;

namespace Qweree.Authentication.Sdk.Identity
{
    public class ClientDto
    {
        public Guid? Id { get; set; }
        public string? ClientId { get; set; }
        public string? ApplicationName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public Guid? OwnerId { get; set; }
    }
}