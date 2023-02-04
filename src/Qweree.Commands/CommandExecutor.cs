using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Commands.CommandLine;

namespace Qweree.Commands;

public class CommandExecutor
{
    private readonly List<Command> _commands;

    public CommandExecutor(IEnumerable<Command> commands)
    {
        _commands = commands.ToList();
    }

    public async Task<int> ExecuteCommandAsync(string[] args, CancellationToken cancellationToken = new())
    {
        var pipeline = CommandPipelineFactory.CreatePipeline(args);

        var lastResult = -1;
        foreach (var call in pipeline)
        {
            var command = _commands.SingleOrDefault(c => c.CommandPath == call.CommandPath);

            if (command is null)
                return -4;

            var options = call.Options.ToImmutableDictionary(o =>
                o.OptionName, o => o.OptionValues);

            var optionsBag = new OptionsBag(args.ToImmutableArray(), options);
            var task = command.ExecuteFunc.Invoke(optionsBag, cancellationToken);
            lastResult = await task;

            if (lastResult != 0)
                return lastResult;
        }

        return lastResult;
    }
}