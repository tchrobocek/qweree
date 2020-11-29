using System.IO;

namespace Qweree.Cdn.WebApi.Application.Storage
{
    public class StoreObjectInput
    {
        public StoreObjectInput(string slug, string mediaType, Stream stream)
        {
            Slug = slug;
            MediaType = mediaType;
            Stream = stream;
        }

        public string Slug { get; }
        public string MediaType { get; }
        public Stream Stream { get; }
    }
}