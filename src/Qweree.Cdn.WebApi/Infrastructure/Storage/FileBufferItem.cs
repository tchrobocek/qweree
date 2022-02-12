using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Cdn.WebApi.Domain.Storage;

namespace Qweree.Cdn.WebApi.Infrastructure.Storage;

public class FileBufferItem : IBufferItem
{
    private readonly string _sourcePath;

    public FileBufferItem(string sourcePath, long length)
    {
        _sourcePath = sourcePath;
        Length = length;
    }

    public long Length { get; }
    public Task StoreAsync(string targetPath, CancellationToken cancellationToken)
    {
        File.Move(_sourcePath, targetPath);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        File.Delete(_sourcePath);
    }

    public ValueTask DisposeAsync()
    {
        File.Delete(_sourcePath);
        return ValueTask.CompletedTask;
    }
}