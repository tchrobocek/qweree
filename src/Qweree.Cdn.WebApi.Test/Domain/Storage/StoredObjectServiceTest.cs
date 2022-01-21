using System;
using System.IO;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Qweree.AspNet.Application;
using Qweree.Cdn.WebApi.Domain.Storage;
using Qweree.Cdn.WebApi.Test.Fixture.Factories;
using Qweree.Utils;
using Xunit;

namespace Qweree.Cdn.WebApi.Test.Domain.Storage;

[Trait("Category", "Unit test")]
public class StoredObjectServiceTest
{
    [Fact]
    public async Task TestCreateAsync()
    {
        var dateTimeProvider = new DateTimeProvider();
        var storedObjectRepositoryMock = new Mock<IStoredObjectRepository>();
        var service = new StoredObjectService(dateTimeProvider, storedObjectRepositoryMock.Object);

        var stream = new MemoryStream(new byte[] {0x1, 0x2, 0x3, 0x4, 0x5});
        const string slug = "test/object/slug";
        var input = new StoreObjectInput(slug, MediaTypeNames.Application.Octet, stream, true);
        var response = await service.StoreOrReplaceObjectAsync(input);

        Assert.Equal(ResponseStatus.Ok, response.Status);
        var storedObject = response.Payload ?? throw new ArgumentNullException();

        Assert.Equal(new[] {"test", "object", "slug"}, storedObject.Descriptor.Slug);
        Assert.Equal(5, storedObject.Descriptor.Size);
        Assert.Equal(MediaTypeNames.Application.Octet, storedObject.Descriptor.MediaType);
    }

    [Fact]
    public async Task TestReadAsync()
    {
        var dateTimeProvider = new DateTimeProvider();
        var storedObjectRepositoryMock = new Mock<IStoredObjectRepository>();
        var service = new StoredObjectService(dateTimeProvider, storedObjectRepositoryMock.Object);

        using var storedObject =
            new StoredObject(StoredObjectDescriptorFactory.CreateDefault(), new MemoryStream());
        storedObjectRepositoryMock.Setup(m =>
                m.ReadAsync(new[] {"test", "object", "slug"}, It.IsAny<CancellationToken>()))
            .ReturnsAsync(storedObject);

        var input = new ReadObjectInput("/test/object/slug");
        var response = await service.ReadObjectAsync(input);

        Assert.Equal(ResponseStatus.Ok, response.Status);
        var responseObject = response.Payload ?? throw new ArgumentNullException();

        Assert.Same(storedObject, responseObject);
    }
}