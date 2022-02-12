using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Cdn.Sdk.Storage;
using Qweree.Cdn.WebApi.Domain.Storage;

namespace Qweree.Cdn.WebApi.Infrastructure.Storage;

public interface IObjectStorage
{
    Task StoreAsync(IBufferItem bufferItem, StoredObjectDescriptor descriptor, CancellationToken cancellationToken = new());
    Task<Stream> ReadAsync(StoredObjectDescriptor descriptor, CancellationToken cancellationToken = new());
    Task<StorageStats> GetStatsAsync(CancellationToken cancellationToken = new());
    Task DeleteAsync(string[] slug, CancellationToken cancellationToken = new());
}