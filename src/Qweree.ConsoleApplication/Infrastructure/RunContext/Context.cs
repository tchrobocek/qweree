using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Utils;

namespace Qweree.ConsoleApplication.Infrastructure.RunContext
{
    public class Context
    {
        private ContextConfiguration? _configuration;

        public Context(string rootDirectory)
        {
            RootDirectory = rootDirectory;
        }

        public string RootDirectory { get; }

        public async Task<ContextConfiguration> GetConfigurationAsync(CancellationToken cancellationToken = new())
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

            return _configuration = new ContextConfiguration(configDo.Username ?? "", configDo.RefreshToken ?? "");
        }
    }
}