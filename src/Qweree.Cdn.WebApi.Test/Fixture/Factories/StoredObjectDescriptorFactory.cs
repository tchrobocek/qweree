using System;
using System.Net.Mime;
using Qweree.Cdn.Sdk;
using Qweree.Cdn.Sdk.Storage;

namespace Qweree.Cdn.WebApi.Test.Fixture.Factories
{
    public class StoredObjectDescriptorFactory
    {
        public static StoredObjectDescriptor CreateDefault()
        {
            return CreateDefault($"generated/{Guid.NewGuid()}", MediaTypeNames.Application.Octet, 0L);
        }

        public static StoredObjectDescriptor CreateDefault(string path, string mediaType, long size)
        {
            return new(Guid.NewGuid(), SlugHelper.PathToSlug(path), mediaType, size, DateTime.UtcNow, DateTime.UtcNow);
        }
    }
}