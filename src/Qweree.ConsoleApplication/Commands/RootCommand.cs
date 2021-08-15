using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Commands;
using Qweree.ConsoleApplication.Infrastructure.RunContext;
using ICommand = Qweree.ConsoleApplication.Infrastructure.Commands.ICommand;

namespace Qweree.ConsoleApplication.Commands
{
    public class RootCommand : ICommand
    {
        private readonly Infrastructure.RunContext.Context _context;

        public RootCommand(Infrastructure.RunContext.Context context)
        {
            _context = context;
        }

        public string CommandPath => "";
        public async Task<int> ExecuteAsync(OptionsBag optionsBag, CancellationToken cancellationToken = new CancellationToken())
        {
            ContextConfigurationDo config;
            try
            {
                config = await _context.GetConfigurationAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }

            if (optionsBag.Options.TryGetValue("--auth-uri", out var authUri))
            {
                config.AuthUri = authUri.Single();
            }

            if (optionsBag.Options.TryGetValue("--picc-uri", out var piccUri))
            {
                config.PiccUri = piccUri.Single();
            }

            await _context.SetContextAsync(config, cancellationToken: cancellationToken);

            return 0;
        }
    }
}