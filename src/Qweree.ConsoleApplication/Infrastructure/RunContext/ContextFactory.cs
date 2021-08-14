using System;
using System.IO;
using Qweree.Sdk.Http.HttpClient;

namespace Qweree.ConsoleApplication.Infrastructure.RunContext
{
    public static class ContextFactory
    {
        public const string ContextDir = ".qweree";

        public static Context GuessContext(ITokenStorage tokenStorage)
        {
            var mainDir = Directory.GetCurrentDirectory();
            var testDirectories = new[]
            {
                Directory.GetCurrentDirectory(),
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            };

            string? directory = null;

            foreach (var dir in testDirectories)
            {
                var path = Path.Combine(dir, ContextDir);
                if (Directory.Exists(path))
                {
                    directory = path;
                    break;
                }
            }

            directory ??= Path.Combine(mainDir, ContextDir);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return new Context(directory, tokenStorage);
        }
    }
}