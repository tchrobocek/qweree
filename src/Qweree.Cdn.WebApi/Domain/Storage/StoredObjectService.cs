using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.Cdn.Sdk;
using Qweree.Cdn.Sdk.Storage;
using Qweree.Mongo.Exception;
using Qweree.Session;
using Qweree.Utils;

namespace Qweree.Cdn.WebApi.Domain.Storage;

public class StoredObjectService
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IStoredObjectRepository _storedObjectRepository;
    private readonly ISessionStorage _sessionStorage;
    private readonly IStorageBuffer _buffer;

    public StoredObjectService(IDateTimeProvider dateTimeProvider, IStoredObjectRepository storedObjectRepository, ISessionStorage sessionStorage, IStorageBuffer buffer)
    {
        _dateTimeProvider = dateTimeProvider;
        _storedObjectRepository = storedObjectRepository;
        _sessionStorage = sessionStorage;
        _buffer = buffer;
    }

    public async Task<Response<StoredObjectDescriptor>> StoreOrReplaceObjectAsync(StoreObjectInput input,
        CancellationToken cancellationToken = new())
    {
        var slug = PathHelper.PathToSlug(input.Path);

        await using var bufferItem = await _buffer.AddToBufferAsync(input.Stream, cancellationToken);
        await input.Stream.DisposeAsync();

        var created = _dateTimeProvider.UtcNow;
        var isPrivate = input.IsPrivate;
        try
        {
            var existing = await _storedObjectRepository.ReadAsync(slug, cancellationToken);

            if (!input.Force)
                return Response.Fail<StoredObjectDescriptor>("Stored object already exists.");

            if (existing.Descriptor.OwnerId != _sessionStorage.Id)
                return Response.Fail<StoredObjectDescriptor>(new Error("Forbidden.", (int)HttpStatusCode.Forbidden));

            created = existing.Descriptor.CreatedAt;

            if (isPrivate == null)
                isPrivate = existing.Descriptor.IsPrivate;

            await _storedObjectRepository.DeleteAsync(slug, cancellationToken);
        }
        catch (DocumentNotFoundException)
        {
        }

        var descriptor = new StoredObjectDescriptor(Guid.NewGuid(), _sessionStorage.Id, slug, input.MediaType, bufferItem.Length,
            isPrivate ?? true, created, _dateTimeProvider.UtcNow);

        await _storedObjectRepository.StoreAsync(descriptor, bufferItem, cancellationToken);
        return Response.Ok(descriptor);
    }

    public async Task<Response<StoredObject>> ReadObjectAsync(ReadObjectInput input,
        CancellationToken cancellationToken = new())
    {
        var slug = PathHelper.PathToSlug(input.Path);

        StoredObject storedObject;

        try
        {
            storedObject = await _storedObjectRepository.ReadAsync(slug, cancellationToken);
        }
        catch (Exception e)
        {
            return Response.Fail<StoredObject>(e.Message);
        }

        if (storedObject.Descriptor.IsPrivate && _sessionStorage.Id != storedObject.Descriptor.OwnerId)
        {
            return Response.Fail<StoredObject>(new Error("Forbidden.", (int) HttpStatusCode.Forbidden));
        }

        return Response.Ok(storedObject);
    }

    public async Task<Response> DeleteObjectAsync(string path,
        CancellationToken cancellationToken = new())
    {
        var slug = PathHelper.PathToSlug(path);

        try
        {
            var storedObject = await _storedObjectRepository.ReadAsync(slug, cancellationToken);
            if (storedObject.Descriptor.OwnerId != _sessionStorage.Id)
            {
                return Response.Fail<StoredObject>(new Error("Forbidden.", (int)HttpStatusCode.Forbidden));
            }
        }
        catch (DocumentNotFoundException)
        {
            return Response.Ok();
        }
        catch (Exception e)
        {
            return Response.Fail<StoredObject>(e.Message);
        }
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