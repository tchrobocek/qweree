using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.Cdn.Sdk;
using Qweree.Cdn.Sdk.Storage;
using Qweree.Mongo.Exception;
using Qweree.Utils;

namespace Qweree.Cdn.WebApi.Domain.Storage;

public class StoredObjectService
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IStoredObjectRepository _storedObjectRepository;

    public StoredObjectService(IDateTimeProvider dateTimeProvider, IStoredObjectRepository storedObjectRepository)
    {
        _dateTimeProvider = dateTimeProvider;
        _storedObjectRepository = storedObjectRepository;
    }

    public async Task<Response<StoredObject>> StoreOrReplaceObjectAsync(StoreObjectInput input,
        CancellationToken cancellationToken = new())
    {
        var slug = SlugHelper.PathToSlug(input.Path);

        await using var stream = new MemoryStream();
        await input.Stream.CopyToAsync(stream, cancellationToken);
        await input.Stream.DisposeAsync();
        stream.Seek(0, SeekOrigin.Begin);

        var created = _dateTimeProvider.UtcNow;

        try
        {
            var existing = await _storedObjectRepository.ReadAsync(slug, cancellationToken);

            if (!input.Force)
            {
                return Response.Fail<StoredObject>("Stored object already exists.");
            }

            created = existing.Descriptor.CreatedAt;
            await _storedObjectRepository.DeleteAsync(slug, cancellationToken);
        }
        catch (DocumentNotFoundException)
        {
        }

        var descriptor = new StoredObjectDescriptor(Guid.NewGuid(), slug, input.MediaType, stream.Length,
            created, _dateTimeProvider.UtcNow);

        StoredObject storedObject = new(descriptor, stream);
        await _storedObjectRepository.StoreAsync(storedObject, cancellationToken);

        return Response.Ok(storedObject);
    }

    public async Task<Response<StoredObject>> ReadObjectAsync(ReadObjectInput input,
        CancellationToken cancellationToken = new())
    {
        var slug = SlugHelper.PathToSlug(input.Path);

        StoredObject storedObject;

        try
        {
            storedObject = await _storedObjectRepository.ReadAsync(slug, cancellationToken);
        }
        catch (Exception e)
        {
            return Response.Fail<StoredObject>(e.Message);
        }

        return Response.Ok(storedObject);
    }

    public async Task<Response> DeleteObjectAsync(string path,
        CancellationToken cancellationToken = new())
    {
        var slug = SlugHelper.PathToSlug(path);

        try
        {
            await _storedObjectRepository.DeleteAsync(slug, cancellationToken);
        }
        catch (Exception e)
        {
            return Response.Fail<StoredObject>(e.Message);
        }

        return Response.Ok();
    }
}