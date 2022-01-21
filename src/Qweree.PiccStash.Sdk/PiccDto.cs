using System;

namespace Qweree.PiccStash.Sdk;

public class PiccDto
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public long? Size { get; set; }
    public string? MediaType { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}