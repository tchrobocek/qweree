using System.Collections.Immutable;

namespace Qweree.Commands;

public class OptionsBag
{
    public OptionsBag(ImmutableArray<string> args, ImmutableDictionary<string, ImmutableArray<string>> options)
    {
        Args = args;
        Options = options;
    }

    public ImmutableArray<string> Args { get; }
    public ImmutableDictionary<string, ImmutableArray<string>> Options { get; }
}