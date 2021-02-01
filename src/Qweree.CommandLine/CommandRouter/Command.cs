using System;
using System.Threading;
using System.Threading.Tasks;

namespace Qweree.CommandLine.CommandRouter
{
    public class Command
    {
        public Command(string name, string description, Func<CommandRequest, CancellationToken, Task<int>> asyncCommandFunc)
        {
            Name = name;
            Description = description;
            AsyncCommandFunc = asyncCommandFunc;
        }

        public string Name { get; }
        public string Description { get; }
        public Func<CommandRequest, CancellationToken, Task<int>> AsyncCommandFunc { get; }
    }
}