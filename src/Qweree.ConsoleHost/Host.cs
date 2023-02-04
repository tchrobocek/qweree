using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Qweree.ConsoleHost;

public class Host
{
    private ServiceProvider? _serviceProvider;

    public Host()
    {
        ConfigureAction = DefaultConfigureAction;
        ConfigureServicesAction = DefaultConfigureServicesAction;
    }

    public Action<IServiceCollection> ConfigureServicesAction { get; set; }
    public Action<ConsoleApplicationBuilder> ConfigureAction { get; set; }

    public ServiceProvider ServiceProvider => _serviceProvider ?? throw new InvalidOperationException("host is not built yet.");

    private void DefaultConfigureServicesAction(IServiceCollection obj)
    {
    }

    private void DefaultConfigureAction(ConsoleApplicationBuilder obj)
    {
    }

    private async Task<int> RunApplicationAsync(string[] args,
        Func<IServiceProvider, RequestDelegate> buildApplicationFunction,
        CancellationToken cancellationToken = new())
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

    private RequestDelegate BuildApplication(IServiceProvider serviceProvider)
    {
        var builder = new ConsoleApplicationBuilder(serviceProvider);
        ConfigureAction.Invoke(builder);
        return builder.Build();
    }

    public Host Build()
    {
        var collection = new ServiceCollection();
        ConfigureServicesAction(collection);
        _serviceProvider = collection.BuildServiceProvider();
        return this;
    }

    public async Task<int> RunAsync(string[] args, CancellationToken cancellationToken = new())
    {
        var result = await RunApplicationAsync(args, BuildApplication, cancellationToken);

        if (_serviceProvider is not null)
            await _serviceProvider.DisposeAsync();

        return result;
    }
}