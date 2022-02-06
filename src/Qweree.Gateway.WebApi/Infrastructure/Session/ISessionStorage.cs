namespace Qweree.Gateway.WebApi.Infrastructure.Session;

public interface ISessionStorage : IDisposable
{
    Task WriteAsync(string key, Stream data, CancellationToken cancellationToken = new());
    Task<Stream> ReadAsync(string key, CancellationToken cancellationToken = new());
    Task DeleteAsync(string key, CancellationToken cancellationToken = new());
}