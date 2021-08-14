using System;
using System.Threading;
using System.Threading.Tasks;
using Qweree.ConsoleHost;
using Qweree.ConsoleHost.Extensions;

namespace Qweree.ConsoleApplication.Infrastructure.ErrorHandling
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        public async Task NextAsync(ConsoleContext context, RequestDelegate? next,
            CancellationToken cancellationToken = new())
        {
            try
            {
                if (next != null)
                {
                    await next(context, cancellationToken);
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Unhandled exception: " + e.Message);
                Console.ResetColor();
            }
        }
    }
}