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
    public class SttorageClientTest : IClassFixture<CdnAdapterFixture>
    {
        private readonly StorageClient _storageClient;

        public SttorageClientTest(CdnAdapterFixture cdnAdapterFixture)
        {
            var uri = new Uri(new Uri(cdnAdapterFixture.CdnApiUri), "api/v1/storage/");
            var httpClient = cdnAdapterFixture
                .CreateAuthenticatedHttpClientAsync(new PasswordGrantInput(CdnAdapterFixture.TestAdminUsername,
                    CdnAdapterFixture.TestAdminPassword), new ClientCredentials(CdnAdapterFixture.TestClientId,
                    CdnAdapterFixture.TestClientSecret)).GetAwaiter().GetResult();

            httpClient.BaseAddress = uri;
            _storageClient = new StorageClient(httpClient);
        }

        [Fact]
        public async Task TestStoreAndRetrieveAndDelete()
        {
            var slug = $"/test/{Guid.NewGuid()}";

            var bytes = new byte[] {0x1, 0x2, 0x3, 0x4};
            await using var stream = new MemoryStream(bytes);

            {
                var response = await _storageClient.StoreAsync(slug, MediaTypeNames.Application.Octet, stream);
                response.EnsureSuccessStatusCode();
                var descriptorDto = await response.ReadPayloadAsync();
                var descriptor = StoredObjectDescriptorMapper.FromDto(descriptorDto!);
                Assert.Equal(bytes.Length, descriptor.Size);
                Assert.Equal(slug.Trim('/'), string.Join("/", descriptor.Slug));
                Assert.Equal(MediaTypeNames.Application.Octet, descriptor.MediaType);
            }

            {
                using var response = await _storageClient.RetrieveAsync(slug);
                response.EnsureSuccessStatusCode();
                Assert.Equal(bytes, await response.ReadPayloadAsByteArrayAsync());
            }

            {
                using var response = await _storageClient.DeleteAsync(slug);
                response.EnsureSuccessStatusCode();
            }

            {
                using var response = await _storageClient.RetrieveAsync(slug);
                Assert.False(response.IsSuccessful);
            }
        }

        [Fact]
        public async Task TestReplaceAndRetrieve()
        {
            var slug = $"/test/{Guid.NewGuid()}";

            var bytes = new byte[] {0x1, 0x2, 0x3, 0x4};
            await using var stream = new MemoryStream(bytes);

            {
                var response = await _storageClient.StoreAsync(slug, MediaTypeNames.Application.Octet, stream);
                response.EnsureSuccessStatusCode();
                var descriptorDto = await response.ReadPayloadAsync();
                var descriptor = StoredObjectDescriptorMapper.FromDto(descriptorDto!);
                Assert.Equal(bytes.Length, descriptor.Size);
                Assert.Equal(slug.Trim('/'), string.Join("/", descriptor.Slug));
                Assert.Equal(MediaTypeNames.Application.Octet, descriptor.MediaType);
            }

            {
                stream.Seek(0, SeekOrigin.Begin);
                var response = await _storageClient.StoreAsync(slug, MediaTypeNames.Application.Octet, stream, true);
                response.EnsureSuccessStatusCode();
                var descriptorDto = await response.ReadPayloadAsync();
                var descriptor = StoredObjectDescriptorMapper.FromDto(descriptorDto!);
                Assert.Equal(bytes.Length, descriptor.Size);
                Assert.Equal(slug.Trim('/'), string.Join("/", descriptor.Slug));
                Assert.Equal(MediaTypeNames.Application.Octet, descriptor.MediaType);
            }

            {
                using var response = await _storageClient.RetrieveAsync(slug);
                response.EnsureSuccessStatusCode();
                Assert.Equal(bytes, await response.ReadPayloadAsByteArrayAsync());
            }
        }
    }
}