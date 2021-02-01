using System;
using System.Threading;
using System.Threading.Tasks;

namespace Qweree.CommandLine.AspNet.Extensions
{
    public interface IConsoleListener
    {
        Task<int> RunAsync(RequestDelegate next, CancellationToken cancellationToken);
    }

    public class ConsoleListener : IConsoleListener
    {
        private readonly string[] _args;

        public ConsoleListener(string[] args)
        {
            _args = args;
        }

        public async Task<int> RunAsync(RequestDelegate next, CancellationToken cancellationToken)
        {
            await next(new ConsoleContext{Args = _args}, cancellationToken);
            while (true)
            {
                Console.Write("$ ");
                var line = Console.ReadLine();

                if (line == null || line == "exit")
                {
                    return 0;
                }

                var args = line.Split(" ");

                var context = new ConsoleContext
                {
                    Args = args,
                    ReturnCode = 0
                };

                try
                {
                    await next(context, cancellationToken);
                }
                catch (Exception)
                {
                    return -1;
                }
            }
        }
    }

    public static class ConsoleHostExtensions
    {
        public static ConsoleHost UseConsoleListener(this ConsoleHost host, IConsoleListener listener)
        {
            host.RunApplicationAction = async (_, next, cancellationToken) =>
                await listener.RunAsync(next, cancellationToken);
            return host;
        }
    }
}