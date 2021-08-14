using System;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Commands;
using Qweree.ConsoleApplication.Infrastructure.Commands;

namespace Qweree.ConsoleApplication.Commands.Context
{
    public class ContextLocationCommand : ICommand
    {
        private readonly Infrastructure.RunContext.Context _context;
        public string CommandPath => "context loc";

        public ContextLocationCommand(Infrastructure.RunContext.Context context)
        {
            _context = context;
        }

        public Task<int> ExecuteAsync(OptionsBag optionsBag, CancellationToken cancellationToken = new())
        {
            Console.WriteLine(_context.RootDirectory);
            return Task.FromResult(0);
        }
    }
}