using System;
using System.IO;

namespace Qweree.TestUtils.IO
{
    public class TemporaryFolder : IDisposable
    {
        public TemporaryFolder() : this(System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString()))
        {
        }

        public string Path { get; }

        public TemporaryFolder(string path)
        {
            Path = path;

            if (Directory.Exists(path))
                throw new ArgumentException("Directory already exists.");

            Directory.CreateDirectory(path);
        }

        public void Dispose()
        {
            Directory.Delete(Path, true);
        }
    }
}