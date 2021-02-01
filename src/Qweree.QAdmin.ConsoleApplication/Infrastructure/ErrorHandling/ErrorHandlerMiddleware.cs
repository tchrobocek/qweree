#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using Qweree.CommandLine.AspNet;
using Qweree.CommandLine.AspNet.Extensions;

namespace Qweree.ConsoleApplication.Infrastructure.ErrorHandling
{
    public class ErrorHandlerMiddleware : IMiddleware
    {
        public async Task NextAsync(ConsoleContext context, RequestDelegate? next,
            CancellationToken cancellationToken = new())
        {
            try
            {
                if (next != null)
                    await next(context, cancellationToken);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ResetColor();
            }
        }
    }
}