using System;

namespace Qweree.PiccStash.WebApi.Domain.Collections
{
    public class Collection
    {
        public Collection(Guid id, string @namespace, string displayName, Guid ownerId, DateTime modifiedAt, DateTime createdAt)
        {
            Id = id;
            Namespace = @namespace;
            DisplayName = displayName;
            OwnerId = ownerId;
            ModifiedAt = modifiedAt;
            CreatedAt = createdAt;
        }

        public Guid Id { get; }
        public string Namespace { get; }
        public string DisplayName { get; }
        public Guid OwnerId { get; }
        public DateTime ModifiedAt { get; }
        public DateTime CreatedAt { get; }
    }
}