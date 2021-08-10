using System;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Commands;
using Qweree.ConsoleApplication.Infrastructure.Commands;

namespace Qweree.ConsoleApplication.Commands
{
    public class HelloWorldCommand : ICommand
    {
        public string CommandPath => "hello";

        public Task<int> ExecuteAsync(OptionsBag optionsBag, CancellationToken cancellationToken = new())
        {
            Console.WriteLine("Hello world!");
            return Task.FromResult(0);
        }
    }
}