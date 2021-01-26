using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Cdn.Sdk.Storage;

namespace Qweree.Cdn.WebApi.Infrastructure.Storage
{
    public interface IObjectStorage
    {
        Task StoreAsync(Stream stream, StoredObjectDescriptor descriptor,
            CancellationToken cancellationToken = new());

        Task<Stream> ReadAsync(StoredObjectDescriptor descriptor,
            CancellationToken cancellationToken = new());
    }
}