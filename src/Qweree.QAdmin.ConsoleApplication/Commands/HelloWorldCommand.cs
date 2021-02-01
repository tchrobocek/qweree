using System;
using System.Threading;
using System.Threading.Tasks;
using Qweree.CommandLine.AspNet.CommandRouter;
using Qweree.CommandLine.CommandRouter;

namespace Qweree.ConsoleApplication.Commands
{
    public class HelloWorldCommand : ICommand
    {
        public void Configure(ConfigurationBuilder config)
        {
            config.Name = "hello";
        }

        public Task<int> ExecuteAsync(CommandRequest request, CancellationToken cancellationToken = new())
        {
            Console.WriteLine("Hello World!");
            return Task.FromResult(0);
        }
    }
}