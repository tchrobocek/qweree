using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.Cdn.Sdk;
using Qweree.Cdn.Sdk.Storage;
using Qweree.Cdn.WebApi.Domain.Storage;
using Qweree.Utils;

namespace Qweree.Cdn.WebApi.Application.Storage
{
    public class StoredObjectService
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IStoredObjectRepository _storedObjectRepository;

        public StoredObjectService(IDateTimeProvider dateTimeProvider, IStoredObjectRepository storedObjectRepository)
        {
            _dateTimeProvider = dateTimeProvider;
            _storedObjectRepository = storedObjectRepository;
        }

        public async Task<Response<StoredObject>> StoreObjectAsync(StoreObjectInput input,
            CancellationToken cancellationToken = new())
        {
            var slug = SlugHelper.PathToSlug(input.Path);

            await using var stream = new MemoryStream();
            await input.Stream.CopyToAsync(stream, cancellationToken);
            await input.Stream.DisposeAsync();
            stream.Seek(0, SeekOrigin.Begin);

            var descriptor = new StoredObjectDescriptor(Guid.NewGuid(), slug, input.MediaType, stream.Length,
                _dateTimeProvider.UtcNow, _dateTimeProvider.UtcNow);

            var storedObject = new StoredObject(descriptor, stream);

            try
            {
                await _storedObjectRepository.StoreAsync(storedObject, cancellationToken);
            }
            catch (Exception e)
            {
                return Response.Fail<StoredObject>(e.Message);
            }

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
    }
}