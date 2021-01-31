using System;
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
        }

        public Action<IServiceCollection>? ConfigureServicesAction { get; init; }
        public Action<ConsoleApplicationBuilder>? ConfigureAction { get; init; }

        public async Task<int> RunAsync(string[] args)
        {
            ConfigureServicesAction?.Invoke(_serviceCollection);

            await using var provider = _serviceCollection.BuildServiceProvider();
            using var scope = provider.CreateScope();

            var builder = new ConsoleApplicationBuilder(scope.ServiceProvider);
            ConfigureAction?.Invoke(builder);

            var applicationAction = builder.Build();
            var context = new ConsoleContext {Args = args};
            await applicationAction(context);

            return context.ReturnCode;
        }
    }
}