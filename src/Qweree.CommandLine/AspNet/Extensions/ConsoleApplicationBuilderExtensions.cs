using System.Threading;
using System.Threading.Tasks;

namespace Qweree.CommandLine.AspNet.Extensions
{
    public interface IMiddleware
    {
        Task NextAsync(ConsoleContext context, RequestDelegate? next, CancellationToken cancellationToken = new());
    }

    public static class ConsoleApplicationBuilderExtensions
    {
        public static void UseMiddleware(this ConsoleApplicationBuilder app, IMiddleware middleware)
        {
            app.Use(next =>
            {
                return async (context, token) =>
                {
                    await middleware.NextAsync(context, next, token);
                };
            });
        }
    }
}