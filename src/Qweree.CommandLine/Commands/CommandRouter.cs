using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Qweree.CommandLine.Commands
{
    public class CommandRouter
    {
        private readonly List<Command> _commands = new();

        public CommandRouter(IEnumerable<Command> commands)
        {
            _commands.AddRange(commands);
        }

        public async Task<int> ExecuteAsync(string[] args, CancellationToken cancellationToken = new())
        {
            if (args.Length == 0)
                return 0;

            var command = _commands.FirstOrDefault(c => c.Name == args[0]);

            if (command == null)
            {
                Console.WriteLine("Command not found.");
                return -1;
            }

            return await command.AsyncCommandFunc(new CommandRequest(), cancellationToken);
        }
    }
}