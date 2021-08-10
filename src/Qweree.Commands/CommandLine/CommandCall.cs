using System.Collections.Immutable;

namespace Qweree.Commands.CommandLine
{
    public class CommandCall
    {
        public CommandCall(string commandPath, ImmutableArray<CommandCallOption> options)
        {
            CommandPath = commandPath;
            Options = options;
        }

        public string CommandPath { get; }
        public ImmutableArray<CommandCallOption> Options { get; }
    }

    public class CommandCallOption
    {
        public CommandCallOption(string optionName, ImmutableArray<string> optionValues)
        {
            OptionName = optionName;
            OptionValues = optionValues;
        }

        public string OptionName { get; }
        public ImmutableArray<string> OptionValues { get; }
    }
}