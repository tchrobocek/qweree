using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Qweree.Cdn.WebApi.Domain.Storage;

public interface IStorageBuffer
{
    Task<IBufferItem> AddToBufferAsync(Stream stream, CancellationToken cancellationToken = new());
}

