using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Cdn.WebApi.Domain.Storage;

namespace Qweree.Cdn.WebApi.Infrastructure.Storage
{
    public class FileObjectStorage : IObjectStorage
    {
        private readonly string _rootPath;

        public FileObjectStorage(string rootPath)
        {
            _rootPath = rootPath;
        }

        private string GetPath(StoredObjectDescriptor descriptor)
        {
            var parts = new List<string>
            {
                _rootPath
            };
            parts.AddRange(descriptor.Slug);

            return Path.Combine(parts.ToArray());
        }

        public async Task StoreAsync(Stream stream, StoredObjectDescriptor descriptor, CancellationToken cancellationToken = new CancellationToken())
        {
            var path = GetPath(descriptor);

            if (File.Exists(path) || Directory.Exists(path))
                throw new ArgumentException("Object already exists.");

            var root = Path.GetDirectoryName(path);

            if (!Directory.Exists(root))
                Directory.CreateDirectory(root!);

            await using var fileStream = File.Create(path);
            await stream.CopyToAsync(fileStream, cancellationToken);
        }

        public Task<Stream> ReadAsync(StoredObjectDescriptor descriptor,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var path = GetPath(descriptor);

            if (!File.Exists(path))
                throw new ArgumentException("File does not exist.");

            var stream = File.OpenRead(path);
            return Task.FromResult((Stream) stream);
        }
    }
}