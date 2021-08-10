using System;
using System.IO;

namespace Qweree.ConsoleApplication.Infrastructure.RunContext
{
    public static class ContextFactory
    {
        private const string ContextDir = ".qweree";

        public static Context GuessContext()
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

            directory ??= mainDir;

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return new Context(directory);
        }
    }
}