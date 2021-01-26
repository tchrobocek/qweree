using System;

namespace Qweree.Cdn.WebApi.Application.Explorer
{
    public class ExplorerFile : IExplorerObject
    {
        public ExplorerFile(Guid id, string path, string mediaType, long size, DateTime modifiedAt, DateTime createdAt)
        {
            Id = id;
            Path = path;
            MediaType = mediaType;
            Size = size;
            ModifiedAt = modifiedAt;
            CreatedAt = createdAt;
        }

        public Guid Id { get; }
        public string MediaType { get; }
        public long Size { get; }
        public DateTime ModifiedAt { get; }
        public DateTime CreatedAt { get; }
        public string Path { get; }
    }
}