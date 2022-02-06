using System.Net.Mime;
using Microsoft.Extensions.Options;
using Qweree.Cdn.Sdk;
using Qweree.Cdn.Sdk.Storage;

namespace Qweree.Gateway.WebApi.Infrastructure.Session;

public class QwereeSessionStorage : ISessionStorage
{
    private readonly StorageClient _storageClient;
    private readonly IOptions<QwereeConfigurationDo> _qwereeConfiguration;

    public QwereeSessionStorage(StorageClient storageClient, IOptions<QwereeConfigurationDo> qwereeConfiguration)
    {
        _storageClient = storageClient;
        _qwereeConfiguration = qwereeConfiguration;
    }

    public async Task WriteAsync(string key, Stream data, CancellationToken cancellationToken = new())
    {
        var response = await _storageClient.StoreAsync(GetPath(key), MediaTypeNames.Application.Octet, data, true, true, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<Stream> ReadAsync(string key, CancellationToken cancellationToken = new())
    {
        var response = await _storageClient.RetrieveAsync(GetPath(key), cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.ReadPayloadAsStreamAsync(cancellationToken);
    }

    public async Task DeleteAsync(string key, CancellationToken cancellationToken = new())
    {
        var response = await _storageClient.DeleteAsync(GetPath(key), cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private string GetPath(string key)
    {
        return PathHelper.Combine(PathHelper.GetClientDataPath(_qwereeConfiguration.Value.ClientId ?? string.Empty), "session", key);
    }

    public void Dispose()
    {
    }
}