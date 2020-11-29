using System;
using Qweree.Cdn.WebApi.Domain.Storage;

namespace Qweree.Cdn.WebApi.Test.Fixture.Factories
{
    public class StoredObjectDescriptorFactory
    {
        public static StoredObjectDescriptor CreateDefault(string name, string mediaType, long size)
        {
            return new StoredObjectDescriptor(Guid.NewGuid(), name.Split(", "), mediaType, size, DateTime.UtcNow,DateTime.UtcNow);
        }
    }
}