using System.Threading;
using System.Threading.Tasks;

namespace Qweree.Commands
{
    public delegate Task<int> CommandExecutionFunc(OptionsBag optionsBag, CancellationToken cancellationToken = new());

    public class Command
    {
        public Command(string commandPath, CommandExecutionFunc executeFunc)
        {
            CommandPath = commandPath;
            ExecuteFunc = executeFunc;
        }

        public string CommandPath { get; }
        public CommandExecutionFunc ExecuteFunc { get; }
    }
}