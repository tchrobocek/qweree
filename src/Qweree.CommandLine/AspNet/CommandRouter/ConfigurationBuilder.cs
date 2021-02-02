using System.Collections.Immutable;
using System.Linq;

namespace Qweree.CommandLine.AspNet.CommandRouter
{
    public class ConfigurationBuilder
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";

        public ImmutableArray<ArgumentConfigurationBuilder> Arguments { get; set; } =
            ImmutableArray<ArgumentConfigurationBuilder>.Empty;

        public ImmutableArray<OptionConfigurationBuilder> Options { get; set; } =
            ImmutableArray<OptionConfigurationBuilder>.Empty;

        public CommandConfiguration Build()
        {
            return new(Name, Description, Arguments.Select(a => a.Build()).ToImmutableArray(),
                Options.Select(o => o.Build()).ToImmutableArray());
        }
    }

    public class ArgumentConfigurationBuilder
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public bool Required { get; set; }
        public int Order { get; set; }

        public ArgumentConfiguration Build()
        {
            return new(Name, Description, Required, Order);
        }
    }

    public class OptionConfigurationBuilder
    {
        public string Name { get; set; } = "";
        public char? ShortName { get; set; } = null;
        public string Description { get; set; } = "";
        public bool Required { get; set; }

        public OptionConfiguration Build()
        {
            return new(Name, ShortName, Description, Required);
        }
    }
}