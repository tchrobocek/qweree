using System.Threading;
using System.Threading.Tasks;
using Qweree.Cdn.Sdk.Storage;

namespace Qweree.Cdn.WebApi.Domain.Storage;

public interface IStoredObjectRepository
{
    Task<bool> ExistsAsync(string[] slug, CancellationToken cancellationToken = new());
    Task DeleteAsync(string[] slug, CancellationToken cancellationToken = new());
    Task StoreAsync(StoredObjectDescriptor descriptor, IBufferItem bufferItem, CancellationToken cancellationToken = new());
    Task<StoredObject> ReadAsync(string[] slug, CancellationToken cancellationToken = new());
}