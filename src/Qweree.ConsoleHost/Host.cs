using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Qweree.ConsoleHost
{
    public class Host
    {
        private ServiceProvider? _serviceProvider;

        public Host()
        {
            ConfigureAction = DefaultConfigureAction;
            ConfigureServicesAction = DefaultConfigureServicesAction;
            RunApplicationAction = DefaultRunApplicationAction;
        }

        public Action<IServiceCollection> ConfigureServicesAction { get; set; }
        public Action<ConsoleApplicationBuilder> ConfigureAction { get; set; }
        public Func<string[], Func<IServiceProvider, RequestDelegate>, CancellationToken, Task<int>> RunApplicationAction { get; set; }

        public ServiceProvider ServiceProvider => _serviceProvider ?? throw new InvalidOperationException("host is not built yet.");

        private void DefaultConfigureServicesAction(IServiceCollection obj)
        {
        }

        private void DefaultConfigureAction(ConsoleApplicationBuilder obj)
        {
        }

        private async Task<int> DefaultRunApplicationAction(string[] args, Func<IServiceProvider, RequestDelegate> buildApplicationFunction, CancellationToken cancellationToken = new())
        {
            using var scope = ServiceProvider.CreateScope();
            var next = buildApplicationFunction(scope.ServiceProvider);
            var context = new ConsoleContext
            {
                Args = args
            };
            await next(context, cancellationToken);
            return context.ReturnCode ?? -1;
        }

        public Host Build()
        {
            var collection = new ServiceCollection();
            ConfigureServicesAction(collection);
            _serviceProvider = collection.BuildServiceProvider();
            return this;
        }

        public async Task<int> RunAsync(string[] args)
        {
            var result = await RunApplicationAction(args, BuildApplication, new CancellationToken());

            if (_serviceProvider != null)
                await _serviceProvider.DisposeAsync();

            return result;
        }

        private RequestDelegate BuildApplication(IServiceProvider serviceProvider)
        {
            var builder = new ConsoleApplicationBuilder(serviceProvider);
            ConfigureAction.Invoke(builder);
            return builder.Build();
        }
    }
}