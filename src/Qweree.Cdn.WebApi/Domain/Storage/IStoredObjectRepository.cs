using System.Threading;
using System.Threading.Tasks;

namespace Qweree.Cdn.WebApi.Domain.Storage;

public interface IStoredObjectRepository
{
    Task<bool> ExistsAsync(string[] slug, CancellationToken cancellationToken = new());
    Task DeleteAsync(string[] slug, CancellationToken cancellationToken = new());
    Task StoreAsync(StoredObject storedObject, CancellationToken cancellationToken = new());
    Task<StoredObject> ReadAsync(string[] slug, CancellationToken cancellationToken = new());
}