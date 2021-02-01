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
            serviceCollection.AddScoped(_ => new CommandLine.CommandRouter.CommandRouter(new Command[0]));
            serviceCollection.AddScoped<CommandRouterMiddleware>();
        }

        public static void UseCommandRouter(this ConsoleApplicationBuilder app)
        {
            app.UseMiddleware<CommandRouterMiddleware>();
        }
    }
}