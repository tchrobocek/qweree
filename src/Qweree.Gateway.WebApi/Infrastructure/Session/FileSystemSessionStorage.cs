namespace Qweree.Gateway.WebApi.Infrastructure.Session;

public class FileSystemSessionStorage : ISessionStorage
{
    private readonly string _rootDir;

    public FileSystemSessionStorage(string rootDir)
    {
        _rootDir = rootDir;
    }

    public async Task WriteAsync(string key, Stream data, CancellationToken cancellationToken = new())
    {
        if (!Directory.Exists(_rootDir))
            Directory.CreateDirectory(_rootDir);

        await using var stream = File.OpenWrite(Path.Combine(_rootDir, key));
        await data.CopyToAsync(stream);
    }

    public Task<Stream> ReadAsync(string key, CancellationToken cancellationToken = new())
    {
        var path = Path.Combine(_rootDir, key);

        if (!File.Exists(path))
            throw new ArgumentException("Key is not present in the storage.");

        var stream = File.OpenRead(path);
        return Task.FromResult((Stream) stream);
    }

    public Task DeleteAsync(string key, CancellationToken cancellationToken = new())
    {
        var path = Path.Combine(_rootDir, key);

        if (!File.Exists(path))
            return Task.CompletedTask;

        File.Delete(path);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (Directory.Exists(_rootDir))
            Directory.Delete(_rootDir, true);
    }
}