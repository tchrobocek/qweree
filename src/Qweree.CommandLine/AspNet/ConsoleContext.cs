using System;

namespace Qweree.CommandLine.AspNet
{
    public class ConsoleContext
    {
        public string[] Args { get; set; } = Array.Empty<string>();
        public int ReturnCode { get; set; } = 0;
    }

}