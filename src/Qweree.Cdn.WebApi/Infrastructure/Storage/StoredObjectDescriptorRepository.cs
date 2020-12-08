using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Cdn.Sdk.Storage;
using Qweree.Cdn.WebApi.Domain.Storage;
using Qweree.Mongo;
using Qweree.Mongo.Exception;

namespace Qweree.Cdn.WebApi.Infrastructure.Storage
{
    public class StoredObjectDescriptorRepository : MongoRepositoryBase<StoredObjectDescriptor,StoredObjectDescriptorDo>, IStoredObjectDescriptorRepository
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

        public async Task<StoredObjectDescriptor> GetBySlugAsync(string[] slug, CancellationToken cancellationToken = new CancellationToken())
        {
            var slugInput = string.Join(@""", """, slug);

            if (slug.Any())
                slugInput = "\"" + slugInput + "\"";

            var query = @$"{{""Slug"": {{""$eq"": [{slugInput}]}}}}";
            var descriptor = (await FindAsync(query, cancellationToken)).FirstOrDefault();

            if (descriptor == null)
                throw new DocumentNotFoundException(@$"Descriptor ""{string.Join("/", slug)}"" was not found.");

            return descriptor;
        }
    }
}