using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Cdn.WebApi.Domain.Storage;

namespace Qweree.Cdn.WebApi.Infrastructure.Storage
{
    public interface IObjectStorage
    {
        Task StoreAsync(Stream stream, StoredObjectDescriptor descriptor,
            CancellationToken cancellationToken = new CancellationToken());
        Task<Stream> ReadAsync(StoredObjectDescriptor descriptor,
            CancellationToken cancellationToken = new CancellationToken());
    }
}