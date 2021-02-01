using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Qweree.CommandLine.AspNet.Extensions;
using Qweree.CommandLine.CommandRouter;

namespace Qweree.CommandLine.AspNet.CommandRouter
{
    public class CommandRouterMiddleware : IMiddleware
    {
        private readonly CommandLine.CommandRouter.CommandRouter _commandRouter;
        public CommandRouterMiddleware(CommandLine.CommandRouter.CommandRouter commandRouter)
        {
            _commandRouter = commandRouter;
        }

        public async Task NextAsync(ConsoleContext context, RequestDelegate? next,
            CancellationToken cancellationToken = new())
        {
            var result = await _commandRouter.ExecuteAsync(context.Args, cancellationToken);
            context.ReturnCode = result;

            if (next != null)
                await next(context, cancellationToken);
        }
    }

    public static class CommandRouterExtensions
    {
        public static void AddCommandRouter(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped(p =>
            {
                var commands = p.GetServices<ICommand>();
                return new CommandLine.CommandRouter.CommandRouter(commands.Select(c =>
                {
                    var builder = new ConfigurationBuilder();
                    c.Configure(builder);
                    var configuration = builder.Build();

                    return new Command(configuration.Name, configuration.Description, c.ExecuteAsync);
                }));
            });

            serviceCollection.AddScoped<CommandRouterMiddleware>();
        }

        public static void UseCommandRouter(this ConsoleApplicationBuilder app)
        {
            app.UseMiddleware<CommandRouterMiddleware>();
        }
    }
}