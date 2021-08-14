using System;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Commands;
using Qweree.ConsoleApplication.Infrastructure.Commands;
using Qweree.Utils;

namespace Qweree.ConsoleApplication.Commands.Context
{
    public class ContextReadCommand : ICommand
    {
        private readonly Infrastructure.RunContext.Context _context;
        public string CommandPath => "context read";

        public ContextReadCommand(Infrastructure.RunContext.Context context)
        {
            _context = context;
        }

        public async Task<int> ExecuteAsync(OptionsBag optionsBag, CancellationToken cancellationToken = new())
        {
            try
            {
                var config = await _context.GetConfigurationAsync(cancellationToken);
                Console.WriteLine(JsonUtils.Serialize(config));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }
            return 0;
        }
    }
}