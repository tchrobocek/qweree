using System;
using System.IO;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Cdn.Sdk;
using Qweree.Cdn.Sdk.Storage;
using Qweree.Utils;
using Qweree.WebApplication.Infrastructure.Authentication;

namespace Qweree.WebApplication.Infrastructure.Configuration;

public class ConfigurationService
{
    private readonly StorageClient _storageClient;
    private readonly ClaimsPrincipalStorage _claimsPrincipalStorage;

    public ConfigurationService(StorageClient storageClient, ClaimsPrincipalStorage claimsPrincipalStorage)
    {
        _storageClient = storageClient;
        _claimsPrincipalStorage = claimsPrincipalStorage;
    }

    public async Task<DashboardConfigurationDto> GetDashboardConfigAsync(CancellationToken cancellationToken = new())
    {
        return await GetConfigurationAsync<DashboardConfigurationDto>("dashboard", cancellationToken);
    }

    public async Task<bool> SetDashboardConfigAsync(DashboardConfigurationDto config, CancellationToken cancellationToken = new())
    {
        return await SetConfigurationAsync("dashboard", config, cancellationToken);
    }

    private async Task<TResultType> GetConfigurationAsync<TResultType>(string key,
        CancellationToken cancellationToken = new()) where TResultType : class, new()
    {
        var path = await GetPathAsync(key, cancellationToken);
        using var response = await _storageClient.RetrieveAsync(path, cancellationToken: cancellationToken);

        if (!response.IsSuccessful)
            return new TResultType();

        var stream = await response.ReadPayloadAsStreamAsync(cancellationToken);

        try
        {
            return await JsonUtils.DeserializeAsync<TResultType>(stream, cancellationToken) ?? new TResultType();
        }
        catch (Exception)
        {
            return new TResultType();
        }
    }

    private async Task<bool> SetConfigurationAsync(string key, object configuration,
        CancellationToken cancellationToken = new())
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Invalid key.");

        var path = await GetPathAsync(key, cancellationToken);
        await using var stream = new MemoryStream();
        await JsonUtils.SerializeAsync(stream, configuration, cancellationToken);
        stream.Seek(0, SeekOrigin.Begin);
        var response =
            await _storageClient.StoreAsync(path, MediaTypeNames.Application.Json, stream, true, true,
                cancellationToken);

        return response.IsSuccessful;
    }

    private async Task<string> GetPathAsync(string key, CancellationToken cancellationToken = new())
    {
        var identity = await _claimsPrincipalStorage.GetIdentityAsync(cancellationToken);
        return PathHelper.Combine(PathHelper.GetUserRootPath(identity?.User?.Username!), "config", $"{key}.json");
    }
}