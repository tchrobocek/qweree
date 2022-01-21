using System;
using System.IO;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Cdn.Sdk.Explorer;
using Qweree.Cdn.Sdk.Storage;
using Qweree.Cdn.Sdk.Test.Fixture;
using Xunit;

namespace Qweree.Cdn.Sdk.Test.Explorer;

[Collection("Cdn adapter collection")]
[Trait("Category", "Integration test")]
public class ExplorerClientTest : IClassFixture<CdnAdapterFixture>
{
    private readonly StorageClient _storageClient;
    private readonly ExplorerClient _explorerClient;

    public ExplorerClientTest(CdnAdapterFixture cdnAdapterFixture)
    {
        {
            var uri = new Uri(new Uri(cdnAdapterFixture.CdnApiUri), "api/v1/storage/");
            var httpClient = cdnAdapterFixture
                .CreateAuthenticatedHttpClientAsync(new PasswordGrantInput(CdnAdapterFixture.TestAdminUsername,
                    CdnAdapterFixture.TestAdminPassword), new ClientCredentials(CdnAdapterFixture.TestClientId,
                    CdnAdapterFixture.TestClientSecret)).GetAwaiter().GetResult();

            httpClient.BaseAddress = uri;
            _storageClient = new StorageClient(httpClient);
        }
        {
            var uri = new Uri(new Uri(cdnAdapterFixture.CdnApiUri), "api/v1/explorer/");
            var httpClient = cdnAdapterFixture
                .CreateAuthenticatedHttpClientAsync(new PasswordGrantInput(CdnAdapterFixture.TestAdminUsername,
                    CdnAdapterFixture.TestAdminPassword), new ClientCredentials(CdnAdapterFixture.TestClientId,
                    CdnAdapterFixture.TestClientSecret)).GetAwaiter().GetResult();

            httpClient.BaseAddress = uri;
            _explorerClient = new ExplorerClient(httpClient);
        }
    }

    [Fact]
    public async Task TestStoreAndRetrieve()
    {
        var guid = Guid.NewGuid();

        {
            var slug = $"/test/{guid}/x";
            var bytes = new byte[] {0x1, 0x2, 0x3, 0x4};
            await using var stream = new MemoryStream(bytes);

            var response = await _storageClient.StoreAsync(slug, MediaTypeNames.Application.Octet, stream);
            response.EnsureSuccessStatusCode();
            var descriptorDto = await response.ReadPayloadAsync();
            var descriptor = StoredObjectDescriptorMapper.FromDto(descriptorDto!);
            Assert.Equal(bytes.Length, descriptor.Size);
            Assert.Equal(slug.Trim('/'), string.Join("/", descriptor.Slug));
            Assert.Equal(MediaTypeNames.Application.Octet, descriptor.MediaType);
        }

        {
            using var response = await _explorerClient.ExploreAsync($"/test/{guid}");
            response.EnsureSuccessStatusCode();
            Assert.Single(await response.ReadPayloadAsync(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                Converters = { new ExplorerObjectConverter() }
            }) ?? Array.Empty<IExplorerObjectDto>());
        }
    }
}