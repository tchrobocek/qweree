using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Qweree.CommandLine.AspNet;

namespace Qweree.ConsoleApplication
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
        }

        public static void Configure(ConsoleApplicationBuilder app)
        {
        }

        public static async Task<int> RunApplicationAsync(string[] args, RequestDelegate next, CancellationToken cancellationToken = new())
        {
            while (true)
            {
                var line = Console.ReadLine();

                if (line == null || line == "exit")
                {
                    return 0;
                }

                try
                {
                    var context = new ConsoleContext
                    {
                        Args = args,
                        ReturnCode = 0
                    };

                    await next(context, cancellationToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

        }
    }
}