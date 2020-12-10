using System.IO;

namespace Qweree.Cdn.WebApi.Application.Storage
{
    public class StoreObjectInput
    {
        public StoreObjectInput(string path, string mediaType, long length, Stream stream)
        {
            Path = path;
            MediaType = mediaType;
            Stream = stream;
            Length = length;
        }

        public string Path { get; }
        public string MediaType { get; }
        public Stream Stream { get; }
        public long Length { get; }
    }
}