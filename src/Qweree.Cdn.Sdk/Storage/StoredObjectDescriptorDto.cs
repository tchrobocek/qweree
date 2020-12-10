using System;

namespace Qweree.Cdn.Sdk.Storage
{
    public class StoredObjectDescriptorDto
    {
        public Guid Id { get; set; }
        public string[]? Slug { get; set; }
        public string? MediaType { get; set; }
        public long Size { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}