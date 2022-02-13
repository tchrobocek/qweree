using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.Http;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Utils;

namespace Qweree.ConsoleApplication.Infrastructure.RunContext;

public class Context
{
    private ContextConfigurationDo? _configuration;
    private readonly MemoryTokenStorage? _tokenStorage;

    public Context(string rootDirectory, ITokenStorage tokenStorage)
    {
        RootDirectory = rootDirectory;

        if (tokenStorage is MemoryTokenStorage memoryTokenStorage)
            _tokenStorage = memoryTokenStorage;
    }

    public string RootDirectory { get; private set; }

    public async Task<ContextConfigurationDo> GetConfigurationAsync(CancellationToken cancellationToken = new())
    {
        if (_configuration != null)
            return _configuration;

        var configFilePath = Path.Combine(RootDirectory, "config", "context.json");

        if (!File.Exists(configFilePath))
        {
            throw new InvalidOperationException("Context is not initialized.");
        }

        await using var fileStream = File.OpenRead(configFilePath);
        var configDo = await JsonUtils.DeserializeAsync<ContextConfigurationDo>(fileStream, cancellationToken);

        if (configDo == null)
        {
            throw new InvalidOperationException("Configuration corrupted.");
        }

        return _configuration = configDo;
    }

    public async Task SetContextAsync(ContextConfigurationDo configuration, bool saveContext = false, CancellationToken cancellationToken = new())
    {
        _configuration = configuration;

        if (saveContext)
        {
            await SaveConfigurationAsync(configuration, false, cancellationToken);
        }
    }

    public async Task SaveConfigurationAsync(ContextConfigurationDo configuration, bool isGlobal, CancellationToken cancellationToken = new())
    {
        if (isGlobal)
            RootDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ContextFactory.ContextDir);

        var configFilePath = Path.Combine(RootDirectory, "config", "context.json");

        var dirName = Path.GetDirectoryName(configFilePath);

        if (dirName != null && !Directory.Exists(dirName))
            Directory.CreateDirectory(dirName);

        await using var stream = File.Create(configFilePath);
        await JsonUtils.SerializeAsync(stream, configuration, cancellationToken);
    }

    public async Task SetCredentialsAsync(TokenInfo tokenInfo, CancellationToken cancellationToken = new())
    {
        var configFilePath = Path.Combine(RootDirectory, "config", "rft");

        var dirName = Path.GetDirectoryName(configFilePath);

        if (dirName != null && !Directory.Exists(dirName))
            Directory.CreateDirectory(dirName);

        await File.WriteAllTextAsync(configFilePath, tokenInfo.RefreshToken, cancellationToken);

        if (_tokenStorage != null)
            await _tokenStorage.SetTokenInfoAsync(tokenInfo, cancellationToken);
    }

    public async Task<string> GetRefreshTokenAsync(CancellationToken cancellationToken = new())
    {
        var configFilePath = Path.Combine(RootDirectory, "config", "rft");
        if (!File.Exists(configFilePath))
            throw new InvalidOperationException("User is not logged in.");

        return await File.ReadAllTextAsync(configFilePath, cancellationToken);
    }
}