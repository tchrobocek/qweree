namespace Qweree.Cdn.WebApi.Infrastructure.System
{
    public class MediaTypeStats
    {
        public MediaTypeStats(string mediaType, long count, long usedSpace)
        {
            MediaType = mediaType;
            Count = count;
            UsedSpace = usedSpace;
        }

        public string MediaType { get; }
        public long Count { get; }
        public long UsedSpace { get; }
    }
}