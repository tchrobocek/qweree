using System;

namespace Qweree.Cdn.Sdk.Explorer;

public class ExplorerFileDto : IExplorerObjectDto
{
    public Guid? Id { get; set; }
    public string? MediaType { get; set; }
    public long? Size { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? Path { get; set; }
    public string TypeName => "file";
}