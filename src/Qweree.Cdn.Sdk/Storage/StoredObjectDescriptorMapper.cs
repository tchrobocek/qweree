using System;
using System.Linq;

namespace Qweree.Cdn.Sdk.Storage
{
    public static class StoredObjectDescriptorMapper
    {
        public static StoredObjectDescriptorDto ToDto(StoredObjectDescriptor storedObject)
        {
            return new StoredObjectDescriptorDto
            {
                Id = storedObject.Id,
                Slug = storedObject.Slug.ToArray(),
                Size = storedObject.Size,
                CreatedAt = storedObject.CreatedAt,
                MediaType = storedObject.MediaType,
                ModifiedAt = storedObject.ModifiedAt
            };
        }
        public static StoredObjectDescriptor FromDto(StoredObjectDescriptorDto storedObject)
        {
            return new StoredObjectDescriptor(storedObject.Id,
                storedObject.Slug ?? ArraySegment<string>.Empty, storedObject.MediaType ?? "", storedObject.Size,
                storedObject.CreatedAt, storedObject.ModifiedAt);
        }
    }
}