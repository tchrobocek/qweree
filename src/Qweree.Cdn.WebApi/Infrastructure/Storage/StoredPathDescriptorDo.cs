using System;

namespace Qweree.Cdn.WebApi.Infrastructure.Storage
{
    public class StoredPathDescriptorDo
    {
        public string[]? Id { get; set; }
        public int? TotalCount { get; set; }
        public long? TotalSize { get; set; }
        public DateTime? LastCreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public Guid? FirstId { get; set; }
        public string? FirstMediaType { get; set; }
        public long? FirstSize { get; set; }
        public DateTime? FirstCreatedAt { get; set; }
        public DateTime? FirstModifiedAt { get; set; }
    }
}