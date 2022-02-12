using System.Net.Mime;
using Qweree.Cdn.Sdk.Storage;
using Qweree.Sdk.Http;
using Qweree.Utils;

namespace Qweree.Cdn.Sdk.Extensions;

public static class StorageClientExtensions
{
    public static async Task<ApiResponse> RetrieveAsync(this StorageClient @this, string path, CancellationToken cancellationToken = new())
    {
        return await @this.RetrieveAsync(path, cancellationToken: cancellationToken);
    }

    public static async Task<TResultType?> RetrieveAsObjectAsync<TResultType>(this StorageClient @this, string path,  CancellationToken cancellationToken = new()) where TResultType : class
    {
        using var response = await @this.RetrieveAsync(path, cancellationToken);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.ReadPayloadAsStreamAsync(cancellationToken);
        return await JsonUtils.DeserializeAsync<TResultType>(stream, cancellationToken);
    }

    public static async Task StoreAsObjectAsync(this StorageClient @this, string path, object value, bool force = false, bool isPrivateResource = true,  CancellationToken cancellationToken = new())
    {
        await using var stream = new MemoryStream();
        await JsonUtils.SerializeAsync(stream, value, cancellationToken);
        using var response = await @this.StoreAsync(path, MediaTypeNames.Application.Json, stream, force, isPrivateResource, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}