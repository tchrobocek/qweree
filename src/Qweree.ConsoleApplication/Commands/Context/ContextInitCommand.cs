using System.Linq;
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
            var authUri = "http://qweree.chrobocek.com/auth/";
            var piccUri = "http://qweree.chrobocek.com/picc/";

            if (optionsBag.Options.TryGetValue("--auth-uri", out var authUris))
            {
                authUri = authUris.Single();
            }

            if (optionsBag.Options.TryGetValue("--picc-uri", out var piccUris))
            {
                piccUri = piccUris.Single();
            }

            var config = new ContextConfigurationDo
            {
                AuthUri = authUri,
                PiccUri = piccUri
            };

            var isGlobal = optionsBag.Options.TryGetValue("-g", out _);
            await _context.SaveConfigurationAsync(config, isGlobal, cancellationToken);
            return 0;
        }
    }
}