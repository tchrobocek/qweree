using System;

namespace Qweree.Cdn.WebApi.Infrastructure.Storage;

public class StoredObjectDescriptorDo
{
    public Guid? Id { get; set; }
    public Guid? OwnerId { get; set; }
    public string[]? Slug { get; set; }
    public string? MediaType { get; set; }
    public long? Size { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}