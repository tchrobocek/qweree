using System;

namespace Qweree.Cdn.Sdk.Explorer;

public class ExplorerDirectoryDto : IExplorerObjectDto
{
    public long? TotalCount { get; set; }
    public long? TotalSize { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? Path { get; set; }
    public string TypeName => "directory";
}