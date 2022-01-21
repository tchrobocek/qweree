using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Cdn.WebApi.Infrastructure.Storage;

namespace Qweree.Cdn.WebApi.Infrastructure.System;

public class StatsService
{
    private readonly IObjectStorage _objectStorage;
    private readonly IStoredObjectDescriptorRepository _storedObjectDescriptorRepository;

    public StatsService(IStoredObjectDescriptorRepository storedObjectDescriptorRepository, IObjectStorage objectStorage)
    {
        _storedObjectDescriptorRepository = storedObjectDescriptorRepository;
        _objectStorage = objectStorage;
    }

    public async Task<CdnStats> ComputeCdnStatsAsync(CancellationToken cancellationToken = new())
    {
        var dbStats = (await _storedObjectDescriptorRepository.GetObjectsStatsAsync(cancellationToken))
            .ToArray();
        var storageStats = await _objectStorage.GetStatsAsync(cancellationToken);

        return new CdnStats(storageStats.TotalSpace, storageStats.AvailableSpace, dbStats
            .Select(s => new MediaTypeStats(s.Id ?? string.Empty, s.Count ?? 0L, s.Used ?? 0L))
            .ToImmutableArray());
    }
}