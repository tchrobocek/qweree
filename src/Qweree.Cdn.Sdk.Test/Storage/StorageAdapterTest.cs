using System;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Cdn.Sdk.Storage;
using Qweree.Cdn.Sdk.Test.Fixture;
using Xunit;

namespace Qweree.Cdn.Sdk.Test.Storage
{
    [Collection("Cdn adapter collection")]
    [Trait("Category", "Integration test")]
    public class StorageAdapterTest : IClassFixture<CdnAdapterFixture>
    {
        private readonly StorageAdapter _cdnAdapter;

        public StorageAdapterTest(CdnAdapterFixture cdnAdapterFixture)
        {
            var uri = new Uri(cdnAdapterFixture.CdnApiUri);
            _cdnAdapter = new StorageAdapter(new Uri(uri, "/api/v1/storage"),
                cdnAdapterFixture
                    .CreateAuthenticatedHttpClientAsync(new PasswordGrantInput(CdnAdapterFixture.TestAdminUsername,
                        CdnAdapterFixture.TestAdminPassword), new ClientCredentials(CdnAdapterFixture.TestClientId,
                        CdnAdapterFixture.TestClientSecret)).GetAwaiter().GetResult());
        }

        [Fact]
        public async Task TestStoreAndRetrieve()
        {
            var slug = $"/test/{Guid.NewGuid()}";

            var bytes = new byte[] {0x1, 0x2, 0x3, 0x4};
            await using var stream = new MemoryStream(bytes);
            var descriptor = await _cdnAdapter.StoreAsync(slug, MediaTypeNames.Application.Octet, stream);
            Assert.Equal(bytes.Length, descriptor.Size);
            Assert.Equal(slug.Trim('/'), string.Join("/", descriptor.Slug));
            Assert.Equal(MediaTypeNames.Application.Octet, descriptor.MediaType);

            await using var actualStream = await _cdnAdapter.RetrieveAsync(slug);
            var actualBytes = new BinaryReader(actualStream).ReadBytes((int) actualStream.Length);

            Assert.Equal(bytes, actualBytes);
        }
    }
}