using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Utils;

namespace Qweree.ConsoleApplication.Infrastructure.RunContext
{
    public class Context
    {
        private ContextConfigurationDo? _configuration;

        public Context(string rootDirectory)
        {
            RootDirectory = rootDirectory;
        }

        public string RootDirectory { get; }

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
                await SaveConfigurationAsync(configuration, cancellationToken);
            }
        }

        public async Task SaveConfigurationAsync(ContextConfigurationDo configuration, CancellationToken cancellationToken = new())
        {
            var configFilePath = Path.Combine(RootDirectory, "config", "context.json");

            var dirName = Path.GetDirectoryName(configFilePath);

            if (dirName != null && !Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);

            await using var stream = File.Create(configFilePath);
            await JsonUtils.SerializeAsync(stream, configuration, cancellationToken);
        }

    }
}