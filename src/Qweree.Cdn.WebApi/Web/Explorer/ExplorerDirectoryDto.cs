using System;

namespace Qweree.Cdn.WebApi.Web.Explorer
{
    public class ExplorerDirectoryDto : IExplorerObjectDto
    {
        public long? TotalCount { get; set; }
        public long? TotalSize { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? Path { get; set; }
    }
}