using System;

namespace Qweree.ConsoleHost;

public class ConsoleContext
{
    public string[] Args { get; set; } = Array.Empty<string>();
    public int? ReturnCode { get; set; }
}