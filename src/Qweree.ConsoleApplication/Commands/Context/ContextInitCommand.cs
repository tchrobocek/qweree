using System.Threading;
using System.Threading.Tasks;
using Qweree.Commands;
using Qweree.ConsoleApplication.Infrastructure.Commands;
using Qweree.ConsoleApplication.Infrastructure.RunContext;

namespace Qweree.ConsoleApplication.Commands.Context
{
    public class ContextInitCommand : ICommand
    {
        private readonly Infrastructure.RunContext.Context _context;
        public string CommandPath => "context init";

        public ContextInitCommand(Infrastructure.RunContext.Context context)
        {
            _context = context;
        }

        public async Task<int> ExecuteAsync(OptionsBag optionsBag, CancellationToken cancellationToken = new())
        {
            var config = new ContextConfigurationDo
            {
                AuthUri = "http://qweree.chrobocek.com/auth/",
                PiccUri = "http://qweree.chrobocek.com/picc/"
            };

            await _context.SaveConfigurationAsync(config, cancellationToken);
            return 0;
        }
    }
}