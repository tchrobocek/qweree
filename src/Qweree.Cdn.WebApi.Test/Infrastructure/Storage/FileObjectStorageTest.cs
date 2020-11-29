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
        public async Task TestStoreAndRead()
        {
            var descriptor =
                StoredObjectDescriptorFactory.CreateDefault("test/slug/1", MediaTypeNames.Application.Octet, 0L);

            var bytes = new byte[] {0x1, 0x2, 0x3};
            await using var stream = new MemoryStream(bytes);
            await _fileStorage.StoreAsync(stream, descriptor);

            await using var actualStream = await _fileStorage.ReadAsync(descriptor);
            var actualBytes = new BinaryReader(actualStream).ReadBytes((int) actualStream.Length);

            Assert.Equal(bytes, actualBytes);
        }

        public void Dispose()
        {
            _tempFolder.Dispose();
        }
    }
}