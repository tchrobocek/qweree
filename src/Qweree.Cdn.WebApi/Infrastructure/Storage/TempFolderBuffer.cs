using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Cdn.WebApi.Domain.Storage;

namespace Qweree.Cdn.WebApi.Infrastructure.Storage;

public class TempFolderBuffer : IStorageBuffer
{
    private readonly string _tempRoot;

    public TempFolderBuffer(string tempRoot)
    {
        _tempRoot = tempRoot;
    }

    public async Task<IBufferItem> AddToBufferAsync(Stream stream, CancellationToken cancellationToken = new())
    {
        var path = Path.Combine(_tempRoot, Guid.NewGuid().ToString());
        long length;
        await using (var file = File.OpenWrite(path))
        {
            await stream.CopyToAsync(file, cancellationToken);
            await stream.FlushAsync(cancellationToken);
            length = file.Length;
        }

        return new FileBufferItem(path, length);
    }
}