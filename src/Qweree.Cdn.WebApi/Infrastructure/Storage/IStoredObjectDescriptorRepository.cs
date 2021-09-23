using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Cdn.Sdk.Storage;
using Qweree.Cdn.WebApi.Domain.Persistence;

namespace Qweree.Cdn.WebApi.Infrastructure.Storage
{
    public interface IStoredObjectDescriptorRepository : IRepository<StoredObjectDescriptor>
    {
        Task<StoredObjectDescriptor> GetBySlugAsync(string[] slug, CancellationToken cancellationToken = new());
        Task<IEnumerable<StoredPathDescriptorDo>> FindInSlugAsync(string[] slug,
            CancellationToken cancellationToken = new());
        Task<IEnumerable<StoredObjectStatsDo>> GetObjectsStatsAsync(CancellationToken cancellationToken = new());
    }
}