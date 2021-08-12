using System;

namespace Qweree.PiccStash.WebApi.Domain.Collections
{
    public class CollectionItem
    {
        public CollectionItem(Guid id, Guid collectionId, Guid storedItemId, DateTime createdAt, DateTime modifiedAt)
        {
            Id = id;
            CollectionId = collectionId;
            StoredItemId = storedItemId;
            CreatedAt = createdAt;
            ModifiedAt = modifiedAt;
        }

        public Guid Id { get; }
        public Guid CollectionId { get; }
        public Guid StoredItemId { get; }
        public DateTime CreatedAt { get; }
        public DateTime ModifiedAt { get; }
    }
}