using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Qweree.Commands.CommandLine
{
    public static class CommandPipelineFactory
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

                if (arg.StartsWith("-") && arg != "--")
                {
                    optionsStarted = true;
                    var optionKeys = new[] {arg};
                    if (!arg.StartsWith("--"))
                    {
                        optionKeys = arg.TrimStart('-').Select(c => '-' + c.ToString()).ToArray();
                    }

                    foreach (var optionKey in optionKeys)
                    {
                        if (!options.ContainsKey(optionKey))
                        {
                            options[optionKey] = new List<string>();
                        }

                        if (nextArg != null && !nextArg.StartsWith("-"))
                        {
                            options[optionKey].Add(nextArg);
                            i++;
                        }
                    }
                }

                if (!optionsStarted && arg != "--")
                {
                    commandPath += arg + " ";
                }

                if (optionsStarted && !arg.StartsWith("-") || i == args.Length - 1 || arg == "--")
                {
                    if (commandPath == "")
                    {
                        continue;
                    }

                    commandPath = commandPath[..^1];
                    var callOptions = options.Select(kv =>
                        new CommandCallOption(kv.Key, kv.Value.ToImmutableArray()));
                    calls.Add(new CommandCall(commandPath, callOptions.ToImmutableArray()));

                    commandPath = arg.TrimStart('-');
                    if (!string.IsNullOrWhiteSpace(commandPath))
                        commandPath += " ";

                    options = new Dictionary<string, List<string>>();
                    optionsStarted = false;
                }
            }

            return calls;
        }
    }
}