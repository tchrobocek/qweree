using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Cdn.Sdk.Storage;
using Qweree.Cdn.WebApi.Domain.Storage;

namespace Qweree.Cdn.WebApi.Infrastructure.Storage;

public class FileObjectStorage : IObjectStorage
{
    private readonly string _rootPath;

    public FileObjectStorage(string rootPath)
    {
        _rootPath = rootPath;
    }

    public async Task StoreAsync(IBufferItem bufferItem, StoredObjectDescriptor descriptor,
        CancellationToken cancellationToken = new())
    {
        var path = GetPath(descriptor);

        if (File.Exists(path) || Directory.Exists(path))
            throw new ArgumentException("Object already exists.");

        var root = Path.GetDirectoryName(path);

        if (!Directory.Exists(root))
            Directory.CreateDirectory(root!);

        await bufferItem.StoreAsync(path, cancellationToken);
    }

    public Task<Stream> ReadAsync(StoredObjectDescriptor descriptor,
        CancellationToken cancellationToken = new())
    {
        var path = GetPath(descriptor);

        if (!File.Exists(path))
            throw new ArgumentException("File does not exist.");

        var stream = File.OpenRead(path);
        return Task.FromResult((Stream) stream);
    }

    public Task<StorageStats> GetStatsAsync(CancellationToken cancellationToken = new())
    {
        var drive = new DriveInfo(Path.GetPathRoot(_rootPath) ?? string.Empty);
        return Task.FromResult(new StorageStats(drive.TotalSize, drive.AvailableFreeSpace));
    }

    public Task DeleteAsync(string[] slug, CancellationToken cancellationToken = new())
    {
        var path = GetPath(slug);

        if (File.Exists(path))
            File.Delete(path);

        return Task.CompletedTask;
    }

    private string GetPath(StoredObjectDescriptor descriptor)
    {
        return GetPath(descriptor.Slug);
    }

    private string GetPath(IEnumerable<string> slug)
    {
        var parts = new List<string>
        {
            _rootPath
        };
        parts.AddRange(slug);

        return Path.Combine(parts.ToArray());
    }
}