using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Cdn.WebApi.Domain.Storage;

namespace Qweree.Cdn.WebApi.Infrastructure.Storage;

public class MemoryBufferItem : IBufferItem
{
    public static async Task<MemoryBufferItem> FromStreamAsync(Stream stream, CancellationToken cancellationToken = new())
    {
        var memoryStream = new MemoryStream();
        await stream.CopyToAsync(stream, cancellationToken);
        await stream.FlushAsync(cancellationToken);
        memoryStream.Seek(0, SeekOrigin.Begin);

        return new MemoryBufferItem(memoryStream);
    }

    private readonly MemoryStream _memoryStream;

    public MemoryBufferItem(MemoryStream memoryStream)
    {
        _memoryStream = memoryStream;
    }

    public long Length => _memoryStream.Length;

    public async Task StoreAsync(string targetPath, CancellationToken cancellationToken)
    {
        await using var file = File.OpenWrite(targetPath);
        await _memoryStream.CopyToAsync(file, cancellationToken);
        await _memoryStream.FlushAsync(cancellationToken);
    }

    public void Dispose()
    {
        _memoryStream.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _memoryStream.DisposeAsync();
    }
}