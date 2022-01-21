using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Qweree.Cdn.Sdk.Storage;

public class StoredObjectDescriptor
{
    public StoredObjectDescriptor(Guid id, IEnumerable<string> slug, string mediaType, long size,
        DateTime createdAt, DateTime modifiedAt)
    {
        Id = id;
        Slug = slug.ToImmutableArray();
        MediaType = mediaType;
        Size = size;
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }

    public Guid Id { get; }
    public ImmutableArray<string> Slug { get; }
    public string MediaType { get; }
    public long Size { get; }
    public DateTime CreatedAt { get; }
    public DateTime ModifiedAt { get; }
}