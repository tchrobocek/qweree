using System;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Cdn.WebApi.Domain.Storage;
using Qweree.Mongo.Exception;

namespace Qweree.Cdn.WebApi.Infrastructure.Storage;

public class StoredObjectRepository : IStoredObjectRepository
{
    private readonly IStoredObjectDescriptorRepository _descriptorRepository;
    private readonly IObjectStorage _objectStorage;

    public StoredObjectRepository(IStoredObjectDescriptorRepository descriptorRepository,
        IObjectStorage objectStorage)
    {
        _descriptorRepository = descriptorRepository;
        _objectStorage = objectStorage;
    }

    public async Task<bool> ExistsAsync(string[] slug, CancellationToken cancellationToken = new())
    {
        try
        {
            await _descriptorRepository.GetBySlugAsync(slug, cancellationToken);
            return true;
        }
        catch (DocumentNotFoundException)
        {
            return false;
        }
    }

    public async Task DeleteAsync(string[] slug, CancellationToken cancellationToken = new())
    {
        await _objectStorage.DeleteAsync(slug, cancellationToken);
        await _descriptorRepository.DeleteBySlugAsync(slug, cancellationToken);
    }

    public async Task StoreAsync(StoredObject storedObject, CancellationToken cancellationToken = new())
    {
        await _descriptorRepository.InsertAsync(storedObject.Descriptor, cancellationToken);

        try
        {
            await _objectStorage.StoreAsync(storedObject.Stream, storedObject.Descriptor, cancellationToken);
        }
        catch (Exception)
        {
            await _descriptorRepository.DeleteOneAsync(storedObject.Descriptor.Id, cancellationToken);
            throw;
        }
    }

    public async Task<StoredObject> ReadAsync(string[] slug, CancellationToken cancellationToken = new())
    {
        var descriptor = await _descriptorRepository.GetBySlugAsync(slug, cancellationToken);
        var stream = await _objectStorage.ReadAsync(descriptor, cancellationToken);

        return new StoredObject(descriptor, stream);
    }
}