using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Qweree.CommandLine.Commands
{
    public class Command
    {
        public Command(string name, ImmutableArray<Argument> arguments, ImmutableArray<Option> options, Func<CommandRequest, CancellationToken, Task<int>> asyncCommandFunc)
        {
            Name = name;
            AsyncCommandFunc = asyncCommandFunc;
            Arguments = arguments;
            Options = options;
        }

        public string Name { get; }
        public ImmutableArray<Argument> Arguments { get; }
        public ImmutableArray<Option> Options { get; }
        public Func<CommandRequest, CancellationToken, Task<int>> AsyncCommandFunc { get; }
    }
}