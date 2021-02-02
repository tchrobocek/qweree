using System.Collections.Immutable;

namespace Qweree.CommandLine.AspNet.Commands
{
    public class Configuration
    {
        public Configuration(string name, string description, ImmutableArray<ArgumentConfiguration> arguments, ImmutableArray<OptionConfiguration> options)
        {
            Name = name;
            Description = description;
            Arguments = arguments;
            Options = options;
        }

        public string Name { get; }
        public string Description { get; }
        public ImmutableArray<ArgumentConfiguration> Arguments { get; }
        public ImmutableArray<OptionConfiguration> Options { get; }
    }

    public class ArgumentConfiguration
    {
        public ArgumentConfiguration(string name, string description, bool required, int order)
        {
            Name = name;
            Description = description;
            Required = required;
            Order = order;
        }

        public string Name { get; }
        public string Description { get; }
        public bool Required { get; }
        public int Order { get; }
    }

    public class OptionConfiguration
    {
        public OptionConfiguration(string name, char? shortName, string description, bool required)
        {
            Name = name;
            ShortName = shortName;
            Description = description;
            Required = required;
        }

        public string Name { get; }
        public char? ShortName { get; }
        public string Description { get; }
        public bool Required { get; }
    }
}