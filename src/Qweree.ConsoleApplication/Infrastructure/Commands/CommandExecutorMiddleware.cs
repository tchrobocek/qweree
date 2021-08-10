using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Commands;
using Qweree.ConsoleHost;
using Qweree.ConsoleHost.Extensions;

namespace Qweree.ConsoleApplication.Infrastructure.Commands
{
    public class CommandExecutorMiddleware : IMiddleware
    {
        private readonly CommandExecutor _executor;

        public CommandExecutorMiddleware(IEnumerable<ICommand> commands)
        {
            _executor = new CommandExecutor(commands.Select(c => new Command(c.CommandPath, c.ExecuteAsync)));
        }

        public async Task NextAsync(ConsoleContext context, RequestDelegate? next,
            CancellationToken cancellationToken = new())
        {
            var returnCode = await _executor.ExecuteCommandAsync(context.Args, cancellationToken);
            context.ReturnCode = returnCode;

            if (next != null)
                await next.Invoke(context, cancellationToken);
        }
    }
}