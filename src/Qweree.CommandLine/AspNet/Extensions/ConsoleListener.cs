using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Qweree.CommandLine.AspNet.Extensions
{
    public interface IConsoleListener
    {
        Task<int> RunAsync(Func<IServiceProvider, RequestDelegate> buildAppFunc, CancellationToken cancellationToken);
    }

    public class ConsoleListener : IConsoleListener
    {
        private readonly string[] _args;
        private readonly IServiceProvider _serviceProvider;

        public ConsoleListener(string[] args, IServiceProvider serviceProvider)
        {
            _args = args;
            _serviceProvider = serviceProvider;
        }

        public async Task<int> RunAsync(Func<IServiceProvider, RequestDelegate> buildAppFunc, CancellationToken cancellationToken)
        {
            var scope = _serviceProvider.CreateScope();
            var next = buildAppFunc(scope.ServiceProvider);
            await next(new ConsoleContext{Args = _args}, cancellationToken);
            scope.Dispose();

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

                scope = _serviceProvider.CreateScope();
                next = buildAppFunc(scope.ServiceProvider);

                await next(context, cancellationToken);
                scope.Dispose();
            }
        }
    }
}