using System;

namespace Qweree.Cdn.WebApi.Web.System
{
    public class CdnStatsDto
    {
        public long? DiskSpaceTotal { get; set; }
        public long? DiskSpaceAvailable { get; set; }
        public long? ItemsCount { get; set; }
        public long? SpaceUsed { get; set; }
        public MediaTypeStatsDto[] MediaTypes { get; set; } = Array.Empty<MediaTypeStatsDto>();
    }
}