using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Qweree.CommandLine.Commands
{
    public static class CommandRequestFactory
    {
        public static CommandRequest FromArgs(string[] args, IEnumerable<Argument> commandArguments, IEnumerable<Option> commandOptions)
        {
            commandOptions = commandOptions.ToArray();
            var cmdArgs = commandArguments.OrderBy(c => c.Order)
                .ToArray();

            var arguments = new Dictionary<string, string>();
            var options = new Dictionary<string, List<string>>();
            var remaining = new List<string>();

            var argumentsEnded = false;
            var argsI = 0;

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (!argumentsEnded)
                {
                    if (arg.StartsWith("-"))
                    {
                        i--;
                        argumentsEnded = true;
                        continue;
                    }

                    if (cmdArgs.Length > argsI)
                    {
                        var cmdArg = cmdArgs[argsI];
                        arguments.Add(cmdArg.Name, arg);
                        argsI++;
                    }
                    else
                    {
                        remaining.Add(arg);
                    }

                    continue;
                }

                if (arg.StartsWith("-"))
                {
                    if (arg.StartsWith("--"))
                    {
                        var optName = arg.Substring("--".Length);
                        var option = commandOptions.FirstOrDefault(o => o.Name == optName);

                        if (option == null)
                        {
                            remaining.Add(arg);
                            continue;
                        }

                        if (!options.ContainsKey(option.Name))
                        {
                            options.Add(option.Name, new List<string>());
                            if (option.ShortName != null)
                            {
                                options.Add(option.ShortName?.ToString()!, new List<string>());
                            }
                        }

                        if (args.Length > i + 1)
                        {
                            var nextArg = args[i + 1];
                            i++;
                            if (!nextArg.StartsWith("-"))
                            {
                                options[option.Name].Add(nextArg);
                                if (option.ShortName != null)
                                {
                                    options[option.ShortName?.ToString()!].Add(nextArg);
                                }
                            }
                        }
                    }
                    else
                    {
                        var opts = arg.Substring("-".Length);
                        Option? option = null;

                        for (var ii = 0; ii < opts.Length; ii++)
                        {
                            option = commandOptions.FirstOrDefault(o => o.ShortName == opts[ii]);

                            if (option == null)
                            {
                                remaining.Add(arg);
                                continue;
                            }

                            if (!options.ContainsKey(option.Name))
                            {
                                options.Add(option.Name, new List<string>());
                                if (option.ShortName != null)
                                {
                                    options.Add(option.ShortName?.ToString()!, new List<string>());
                                }
                            }
                        }

                        if (option != null && args.Length > i + 1)
                        {
                            var nextArg = args[i + 1];
                            i++;
                            if (!nextArg.StartsWith("-"))
                            {
                                options[option.Name].Add(nextArg);
                                if (option.ShortName != null)
                                {
                                    options[option.ShortName?.ToString()!].Add(nextArg);
                                }
                            }
                        }
                    }
                }
                else
                {
                    remaining.Add(arg);
                }
            }

            return new CommandRequest(args.ToImmutableArray(), arguments.ToImmutableDictionary(),
                options.ToDictionary(kv => kv.Key, kv => kv.Value.ToImmutableArray()).ToImmutableDictionary(),
                remaining.ToImmutableArray());
        }
    }
}