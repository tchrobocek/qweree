using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Qweree.CommandLine.AspNet.Extensions
{
    public interface IMiddleware
    {
        Task NextAsync(ConsoleContext context, RequestDelegate? next, CancellationToken cancellationToken = new());
    }

    public static class ConsoleApplicationBuilderExtensions
    {
        public static void UseMiddleware<TMiddlewareType>(this ConsoleApplicationBuilder app) where TMiddlewareType : IMiddleware
        {
            app.Use(next =>
            {
                return async (context, token) =>
                {
                    var middleware = app.ServiceProvider.GetRequiredService<TMiddlewareType>();
                    await middleware.NextAsync(context, next, token);
                };
            });
        }
    }
}