using System.Threading;
using System.Threading.Tasks;
using Qweree.Cdn.Sdk.Storage;
using Qweree.Cdn.WebApi.Domain.Persistence;
using Qweree.Cdn.WebApi.Domain.Storage;

namespace Qweree.Cdn.WebApi.Infrastructure.Storage
{
    public interface IStoredObjectDescriptorRepository : IRepository<StoredObjectDescriptor>
    {
        Task<StoredObjectDescriptor> GetBySlugAsync(string[] slug, CancellationToken cancellationToken);
    }
}