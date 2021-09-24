namespace Qweree.Cdn.WebApi.Infrastructure.Storage
{
    public class StorageStats
    {
        public StorageStats(long totalSpace, long availableSpace)
        {
            TotalSpace = totalSpace;
            AvailableSpace = availableSpace;
        }

        public long TotalSpace { get; }
        public long AvailableSpace { get; }
    }
}