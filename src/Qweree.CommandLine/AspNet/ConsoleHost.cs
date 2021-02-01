using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Qweree.CommandLine.AspNet
{
    public class ConsoleHost
    {
        private readonly IServiceCollection _serviceCollection;

        public ConsoleHost()
        {
            _serviceCollection = new ServiceCollection();
            RunApplicationAction = DefaultConsoleListenAction;
        }

        public Action<IServiceCollection>? ConfigureServicesAction { get; init; }
        public Action<ConsoleApplicationBuilder>? ConfigureAction { get; init; }
        public Func<string[], RequestDelegate, CancellationToken, Task<int>> RunApplicationAction { get; init; }

        private async Task<int> DefaultConsoleListenAction(string[] args, RequestDelegate next, CancellationToken cancellationToken = new())
        {
            var context = new ConsoleContext
            {
                Args = args
            };

            await next(context, cancellationToken);
            return context.ReturnCode;
        }

        public async Task<int> RunAsync(string[] args)
        {
            ConfigureServicesAction?.Invoke(_serviceCollection);

            await using var provider = _serviceCollection.BuildServiceProvider();
            using var scope = provider.CreateScope();

            var builder = new ConsoleApplicationBuilder(scope.ServiceProvider);
            ConfigureAction?.Invoke(builder);

            var applicationAction = builder.Build();
            return await RunApplicationAction(args, applicationAction, new CancellationToken());
        }
    }
}