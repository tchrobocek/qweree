using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Qweree.CommandLine.AspNet.Extensions;
using Qweree.CommandLine.Commands;

namespace Qweree.CommandLine.AspNet.Commands
{
    public class CommandRouterMiddleware : IMiddleware
    {
        private readonly CommandRouter _commandRouter;
        public CommandRouterMiddleware(CommandRouter commandRouter)
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
                return new CommandRouter(commands.Select(c =>
                {
                    var builder = new ConfigurationBuilder();
                    c.Configure(builder);
                    var configuration = builder.Build();

                    var arguments = configuration.Arguments.Select(a => new Argument(a.Name, a.Order));
                    var options = configuration.Options.Select(o => new Option(o.Name, o.ShortName));
                    return new Command(configuration.Name, arguments.ToImmutableArray(), options.ToImmutableArray(), c.ExecuteAsync);
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