using System;

namespace Qweree.Cdn.Sdk.Explorer
{
    public class ExplorerDirectory : IExplorerObject
    {
        public ExplorerDirectory(string path, long totalCount, long totalSize, DateTime createdAt, DateTime modifiedAt)
        {
            Path = path;
            TotalCount = totalCount;
            TotalSize = totalSize;
            CreatedAt = createdAt;
            ModifiedAt = modifiedAt;
        }

        public long TotalCount { get; }
        public long TotalSize { get; }
        public DateTime CreatedAt { get; }
        public DateTime ModifiedAt { get; }

        public string Path { get; }
    }
}