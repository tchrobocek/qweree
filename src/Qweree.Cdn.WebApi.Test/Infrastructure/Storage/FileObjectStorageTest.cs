using System;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Qweree.Cdn.WebApi.Infrastructure.Storage;
using Qweree.Cdn.WebApi.Test.Fixture.Factories;
using Qweree.TestUtils.IO;
using Xunit;

namespace Qweree.Cdn.WebApi.Test.Infrastructure.Storage
{
    public class FileObjectStorageTest : IDisposable
    {
        private readonly TemporaryFolder _tempFolder;
        private readonly FileObjectStorage _fileStorage;

        public FileObjectStorageTest()
        {
            _tempFolder = new TemporaryFolder();
            _fileStorage = new FileObjectStorage(_tempFolder.Path);
        }

        [Fact]
        public async Task TestStore()
        {
            var descriptor =
                StoredObjectDescriptorFactory.CreateDefault("test/slug/1", MediaTypeNames.Application.Octet, 0L);

            await using var stream = new MemoryStream();
            await _fileStorage.StoreAsync(stream, descriptor);

            Assert.True(File.Exists(Path.Combine(_tempFolder.Path, "test", "slug", "1")));
        }

        public void Dispose()
        {
            _tempFolder.Dispose();
        }
    }
}