using System;

namespace Qweree.Cdn.Sdk.Explorer;

public interface IExplorerObjectDto
{
    string? Path { get; set; }
    DateTime? CreatedAt { get; set; }
    DateTime? ModifiedAt { get; set; }
    string TypeName { get; }
}