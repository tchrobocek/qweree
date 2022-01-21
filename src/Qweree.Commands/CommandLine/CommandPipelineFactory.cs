using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Qweree.Commands.CommandLine;

public static class CommandPipelineFactory
{
    public static IEnumerable<CommandCall> CreatePipeline(string[] args)
    {
        var calls = new List<CommandCall>();

        Dictionary<string, List<string>> options = new();
        var commandPath = new List<string>();
        var optionsStarted = false;

        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.IsNullOrWhiteSpace(arg))
                continue;

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
                commandPath.Add(arg);
            }

            if (optionsStarted && !arg.StartsWith("-") || i == args.Length - 1 || arg == "--")
            {
                var callOptions = options.Select(kv =>
                    new CommandCallOption(kv.Key, kv.Value.ToImmutableArray()));
                calls.Add(new CommandCall(string.Join(" ", commandPath), callOptions.ToImmutableArray()));

                if (arg != "--")
                    commandPath.Add(arg);

                options = new Dictionary<string, List<string>>();
                optionsStarted = false;
            }
        }

        return calls;
    }
}