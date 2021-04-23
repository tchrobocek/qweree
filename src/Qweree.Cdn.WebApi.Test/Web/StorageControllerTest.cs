using System;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Qweree.Cdn.Sdk.Storage;
using Qweree.Cdn.WebApi.Test.Fixture;
using Qweree.Utils;
using Xunit;

namespace Qweree.Cdn.WebApi.Test.Web
{
    [Collection("Web api collection")]
    [Trait("Category", "Integration test")]
    [Trait("Category", "Web api test")]
    public class StorageControllerTest : IClassFixture<WebApiFactory>, IDisposable
    {
        private readonly WebApiFactory _factory;
        private readonly IServiceScope _scope;

        public StorageControllerTest(WebApiFactory factory)
        {
            _factory = factory;
            _scope = factory.Services.CreateScope();
        }

        public void Dispose()
        {
            _factory.Dispose();
            _scope.Dispose();
        }

        [Fact]
        public async Task TestStoreAndRead()
        {
            var client = await _factory.CreateAuthenticatedClientAsync("admin", "password");
            const string text = "Ahoj!";

            {
                await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
                var message = new HttpRequestMessage(HttpMethod.Post, "/api/v1/storage/test/object.txt")
                {
                    Content = new StreamContent(stream)
                    {
                        Headers =
                        {
                            {HeaderNames.ContentType, MediaTypeNames.Text.Plain}
                        }
                    }
                };
                var response = await client.SendAsync(message);
                response.EnsureSuccessStatusCode();

                var descriptorDto = await response.Content.ReadAsObjectAsync<StoredObjectDescriptorDto>();
                Assert.Equal(stream.Length, descriptorDto?.Size!);
                Assert.Equal(new[] {"test", "object.txt"}, descriptorDto?.Slug!);
                Assert.Equal(MediaTypeNames.Text.Plain, descriptorDto?.MediaType!);
            }

            {
                var response = await client.GetAsync("/api/v1/storage/test/object.txt");
                response.EnsureSuccessStatusCode();
                Assert.Equal(MediaTypeNames.Text.Plain, response.Content.Headers.ContentType?.MediaType);
                Assert.Equal(text, await response.Content.ReadAsStringAsync());
            }
        }
    }
}