using System;
using Qweree.Cdn.Sdk;
using Qweree.Cdn.WebApi.Infrastructure.Storage;

namespace Qweree.Cdn.WebApi.Infrastructure.Explorer;

public static class ExplorerObjectMapper
{
    public static IExplorerObject ToExplorerObject(StoredPathDescriptorDo descriptor)
    {
        if (descriptor.TotalCount == 1)
            return new ExplorerFile(descriptor.FirstId ?? Guid.Empty,
                PathHelper.SlugToPath(descriptor.FirstSlug ?? Array.Empty<string>()),
                descriptor.FirstMediaType ?? "", descriptor.FirstSize ?? 0,
                descriptor.FirstModifiedAt ?? DateTime.MinValue, descriptor.FirstCreatedAt ?? DateTime.MinValue);

        if (descriptor.TotalCount > 1)
            return new ExplorerDirectory(PathHelper.SlugToPath(descriptor.Id ?? Array.Empty<string>()),
                descriptor.TotalCount ?? 0, descriptor.TotalSize ?? 0, descriptor.MinCreatedAt ?? DateTime.MinValue,
                descriptor.MaxModifiedAt ?? DateTime.MinValue);

        throw new ArgumentOutOfRangeException(nameof(descriptor.TotalCount),
            "Total count cannot be neither 0 or lower.");
    }
}