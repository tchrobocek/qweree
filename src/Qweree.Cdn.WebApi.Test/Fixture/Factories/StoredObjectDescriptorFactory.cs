using System;
using System.Net.Mime;
using Qweree.Cdn.WebApi.Domain.Storage;

namespace Qweree.Cdn.WebApi.Test.Fixture.Factories
{
    public class StoredObjectDescriptorFactory
    {
        public static StoredObjectDescriptor CreateDefault()
        {
            return CreateDefault($"generated/{Guid.NewGuid()}", MediaTypeNames.Application.Octet, 0L);
        }

        public static StoredObjectDescriptor CreateDefault(string name, string mediaType, long size)
        {
            return new StoredObjectDescriptor(Guid.NewGuid(), name.Split("/", StringSplitOptions.RemoveEmptyEntries), mediaType, size, DateTime.UtcNow,DateTime.UtcNow);
        }
    }
}