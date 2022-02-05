using System.IO;

namespace Qweree.Cdn.WebApi.Domain.Storage;

public class StoreObjectInput
{
    public StoreObjectInput(string path, string mediaType, Stream stream, bool force, bool? isPrivate)
    {
        Path = path;
        MediaType = mediaType;
        Stream = stream;
        Force = force;
        IsPrivate = isPrivate;
    }

    public string Path { get; }
    public string MediaType { get; }
    public Stream Stream { get; }
    public bool Force { get; }
    public bool? IsPrivate { get; }
}