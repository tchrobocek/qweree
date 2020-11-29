using System;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Moq;
using Qweree.AspNet.Application;
using Qweree.Cdn.WebApi.Application.Storage;
using Qweree.Cdn.WebApi.Domain.Storage;
using Qweree.Utils;
using Xunit;

namespace Qweree.Cdn.WebApi.Test.Application.Storage
{
    public class StoredObjectServiceTest
    {
        [Fact]
        public async Task TestCreateAsync()
        {
            var dateTimeProvider = new DateTimeProvider();
            var storedObjectRepositoryMock = new Mock<IStoredObjectRepository>();
            var service = new StoredObjectService(dateTimeProvider, storedObjectRepositoryMock.Object);

            var stream = new MemoryStream();
            const string slug = "test/object/slug";
            var input = new StoreObjectInput(slug, MediaTypeNames.Application.Octet, stream);
            var response = await service.StoreObjectAsync(input);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            var storedObject = response.Payload ?? throw new ArgumentNullException();

            Assert.Equal(new[] {"test", "object", "slug"}, storedObject.Descriptor.Slug);
            Assert.Equal(0, storedObject.Descriptor.Size);
            Assert.Equal(MediaTypeNames.Application.Octet, storedObject.Descriptor.MediaType);
            Assert.Same(storedObject.Stream, stream);

        }
    }
}