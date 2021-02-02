using System.Collections.Immutable;

namespace Qweree.CommandLine.Commands
{
    public class CommandRequest
    {
        public CommandRequest(ImmutableArray<string> args, ImmutableDictionary<string, string> arguments,
            ImmutableDictionary<string, ImmutableArray<string>> options, ImmutableArray<string> remainingArgs)
        {
            Args = args;
            Arguments = arguments;
            Options = options;
            RemainingArgs = remainingArgs;
        }

        public ImmutableArray<string> Args { get; }
        public ImmutableDictionary<string, string> Arguments { get; }
        public ImmutableDictionary<string, ImmutableArray<string>> Options { get; }
        public ImmutableArray<string> RemainingArgs { get; }
    }
}