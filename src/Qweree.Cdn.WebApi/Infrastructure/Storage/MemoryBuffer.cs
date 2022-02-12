using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Cdn.WebApi.Domain.Storage;

namespace Qweree.Cdn.WebApi.Infrastructure.Storage;

public class MemoryBuffer : IStorageBuffer
{
    public async Task<IBufferItem> AddToBufferAsync(Stream stream, CancellationToken cancellationToken = new CancellationToken())
    {
        return await MemoryBufferItem.FromStreamAsync(stream, cancellationToken);
    }
}