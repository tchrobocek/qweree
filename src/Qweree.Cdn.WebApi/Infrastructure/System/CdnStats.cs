using System.Collections.Immutable;

namespace Qweree.Cdn.WebApi.Infrastructure.System;

public class CdnStats
{
    public CdnStats(long diskSpaceTotal, long diskSpaceAvailable, ImmutableArray<MediaTypeStats> mediaTypes)
    {
        DiskSpaceTotal = diskSpaceTotal;
        DiskSpaceAvailable = diskSpaceAvailable;
        MediaTypes = mediaTypes;
    }

    public long DiskSpaceTotal { get; }
    public long DiskSpaceAvailable { get; }
    public ImmutableArray<MediaTypeStats> MediaTypes { get; }
}