using System.Threading;
using System.Threading.Tasks;

namespace Qweree.Cdn.WebApi.Domain.Storage
{
    public interface IStoredObjectRepository
    {
        Task StoreAsync(StoredObject storedObject, CancellationToken cancellationToken= new CancellationToken());
        Task<StoredObject> ReadAsync(string[] slug,
            CancellationToken cancellationToken = new CancellationToken());
    }
}