using System;
using System.IO;
using Qweree.Cdn.Sdk.Storage;

namespace Qweree.Cdn.WebApi.Domain.Storage;

public class StoredObject : IDisposable
{
    public StoredObject(StoredObjectDescriptor descriptor, Stream stream)
    {
        Descriptor = descriptor;
        Stream = stream;
    }

    public StoredObjectDescriptor Descriptor { get; }
    public Stream Stream { get; }

    public void Dispose()
    {
        Stream.Dispose();
    }
}