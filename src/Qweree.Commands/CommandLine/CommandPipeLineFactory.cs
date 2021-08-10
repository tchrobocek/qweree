using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Qweree.Commands.CommandLine
{
    public static class CommandPipeLineFactory
    {
        public static IEnumerable<CommandCall> CreatePipeline(string[] args)
        {
            var calls = new List<CommandCall>();

            Dictionary<string, List<string>> options = new();
            var commandPath = "";
            var optionsStarted = false;

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                string? nextArg = null;

                if (i + 1 < args.Length)
                {
                    nextArg = args[i + 1];
                }

                if (arg.StartsWith("-"))
                {
                    optionsStarted = true;

                    if (!options.ContainsKey(arg))
                    {
                        options[arg] = new List<string>();
                    }

                    if (nextArg != null && !nextArg.StartsWith("-"))
                    {
                        options[arg].Add(nextArg);
                        i++;
                    }
                }

                if (!optionsStarted)
                {
                    commandPath += arg + " ";
                }

                if (optionsStarted && !arg.StartsWith("-") || i == args.Length - 1)
                {
                    commandPath = commandPath[..^1];
                    var callOptions = options.Select(kv =>
                        new CommandCallOption(kv.Key, kv.Value.ToImmutableArray()));
                    calls.Add(new CommandCall(commandPath, callOptions.ToImmutableArray()));

                    commandPath = arg + " ";
                    options = new Dictionary<string, List<string>>();
                    optionsStarted = false;
                }
            }

            return calls;
        }
    }
}