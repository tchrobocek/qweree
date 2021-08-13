using System;

namespace Qweree.PiccStash.WebApi.Domain
{
    public class StashedPicc
    {
        public Guid? Id { get; set; }
        public Guid? OwnerId { get; set; }
        public string? Name { get; set; }
        public string[]? StorageSlug { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}