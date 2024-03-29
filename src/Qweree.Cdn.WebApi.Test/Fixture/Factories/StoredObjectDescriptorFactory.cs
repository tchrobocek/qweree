using System;
using System.Net.Mime;
using Qweree.Cdn.Sdk;
using Qweree.Cdn.Sdk.Storage;

namespace Qweree.Cdn.WebApi.Test.Fixture.Factories;

public class StoredObjectDescriptorFactory
{
    public static StoredObjectDescriptor CreateDefault()
    {
        return CreateDefault($"generated/{Guid.NewGuid()}", MediaTypeNames.Application.Octet, 0L);
    }

    public static StoredObjectDescriptor CreateDefault(string path, string mediaType, long size)
    {
        return CreateDefault($"generated/{Guid.NewGuid()}", MediaTypeNames.Application.Octet, 0L, DateTime.UtcNow, DateTime.UtcNow);
    }

    public static StoredObjectDescriptor CreateDefault(string path, string mediaType, long size, DateTime created, DateTime modified)
    {
        return new StoredObjectDescriptor(Guid.NewGuid(), Guid.NewGuid(), PathHelper.PathToSlug(path), mediaType, size, true, created, modified);
    }

    public static StoredObjectDescriptor CreateDefault(DateTime date)
    {
        return CreateDefault($"generated/{Guid.NewGuid()}", MediaTypeNames.Application.Octet, 0L, date, date);
    }
}