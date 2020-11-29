using System;
using System.Linq;
using Qweree.Cdn.WebApi.Domain.Storage;
using Qweree.Mongo;

namespace Qweree.Cdn.WebApi.Infrastructure.Storage
{
    public class StoredObjectDescriptorRepository : MongoRepositoryBase<StoredObjectDescriptor,StoredObjectDescriptorDo>
    {
        public StoredObjectDescriptorRepository(MongoContext context) : base("stored_objects", context)
        {
        }

        protected override Func<StoredObjectDescriptor, StoredObjectDescriptorDo> ToDocument =>
            descriptor => new StoredObjectDescriptorDo
            {
                Id = descriptor.Id,
                Size = descriptor.Size,
                CreatedAt = descriptor.CreatedAt,
                Slug = descriptor.Slug.ToArray(),
                MediaType = descriptor.MediaType,
                ModifiedAt = descriptor.ModifiedAt
            };
        protected override Func<StoredObjectDescriptorDo, StoredObjectDescriptor> FromDocument =>
            descriptor => new StoredObjectDescriptor(descriptor.Id, descriptor.Slug ?? ArraySegment<string>.Empty, descriptor.MediaType ?? "", descriptor.Size, descriptor.CreatedAt, descriptor.ModifiedAt);
    }
}