using System;
using System.IO;
using System.Threading.Tasks;

namespace Qweree.Gateway.WebApi.Infrastructure.Session;

public class SessionStorage : IDisposable
{
    private readonly string _rootDir;

    public SessionStorage(string rootDir)
    {
        _rootDir = rootDir;
    }

    public async Task WriteAsync(string key, Stream data)
    {
        if (!Directory.Exists(_rootDir))
            Directory.CreateDirectory(_rootDir);

        await using var stream = File.OpenWrite(Path.Combine(_rootDir, key));
        await data.CopyToAsync(stream);
    }

    public Stream ReadAsync(string key)
    {
        var path = Path.Combine(_rootDir, key);

        if (!File.Exists(path))
            throw new ArgumentException("Key is not present in the storage.");

        return File.OpenRead(path);
    }

    public void Dispose()
    {
        if (Directory.Exists(_rootDir))
            Directory.Delete(_rootDir, true);
    }
}