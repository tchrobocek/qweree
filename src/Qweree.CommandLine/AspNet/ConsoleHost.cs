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
            ConfigureAction = DefaultConfigureAction;
            ConfigureServicesAction = DefaultConfigureServicesAction;
            RunApplicationAction = DefaultRunApplicationAction;
        }

        public Action<IServiceCollection> ConfigureServicesAction { get; set; }
        public Action<ConsoleApplicationBuilder> ConfigureAction { get; set; }
        public Func<string[], Func<RequestDelegate>, CancellationToken, Task<int>> RunApplicationAction { get; set; }

        private void DefaultConfigureServicesAction(IServiceCollection obj)
        {
        }

        private void DefaultConfigureAction(ConsoleApplicationBuilder obj)
        {
        }

        private async Task<int> DefaultRunApplicationAction(string[] args, Func<RequestDelegate> buildApplicationFunction, CancellationToken cancellationToken = new())
        {
            var next = buildApplicationFunction();
            var context = new ConsoleContext
            {
                Args = args
            };
            await next(context, cancellationToken);
            return context.ReturnCode;
        }

        public async Task<int> RunAsync(string[] args)
        {
            ConfigureServicesAction.Invoke(_serviceCollection);
            return await RunApplicationAction(args, BuildApplication, new CancellationToken());
        }

        private RequestDelegate BuildApplication()
        {
            var provider = _serviceCollection.BuildServiceProvider();
            using var scope = provider.CreateScope();
            var builder = new ConsoleApplicationBuilder(scope.ServiceProvider);
            ConfigureAction.Invoke(builder);
            return builder.Build();
        }
    }
}