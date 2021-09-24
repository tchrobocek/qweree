using System.IO;

namespace Qweree.Cdn.WebApi.Domain.Storage
{
    public class StoreObjectInput
    {
        public StoreObjectInput(string path, string mediaType, Stream stream)
        {
            Path = path;
            MediaType = mediaType;
            Stream = stream;
        }

        public string Path { get; }
        public string MediaType { get; }
        public Stream Stream { get; }
    }
}