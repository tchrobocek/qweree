using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Qweree.Cdn.Sdk.Storage;

public class StoredObjectDescriptor
{
    public StoredObjectDescriptor(Guid id, Guid ownerId, IEnumerable<string> slug, string mediaType, long size,
        bool isPrivate, DateTime createdAt, DateTime modifiedAt)
    {
        Id = id;
        OwnerId = ownerId;
        Slug = slug.ToImmutableArray();
        MediaType = mediaType;
        Size = size;
        IsPrivate = isPrivate;
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }

    public Guid Id { get; }
    public Guid OwnerId { get; }
    public ImmutableArray<string> Slug { get; }
    public string MediaType { get; }
    public long Size { get; }
    public bool IsPrivate { get; }
    public DateTime CreatedAt { get; }
    public DateTime ModifiedAt { get; }
}