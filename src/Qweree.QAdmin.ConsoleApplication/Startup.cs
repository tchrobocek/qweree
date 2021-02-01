#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Qweree.CommandLine.AspNet;
using Qweree.CommandLine.AspNet.Extensions;

namespace Qweree.ConsoleApplication
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
        }

        public static void Configure(ConsoleApplicationBuilder app)
        {
            app.UseMiddleware(new HelloWorldMiddleware());
        }
    }

    public class HelloWorldMiddleware : IMiddleware
    {
        public Task NextAsync(ConsoleContext context, RequestDelegate? next,
            CancellationToken cancellationToken = new())
        {
            Console.WriteLine("Hello world!");
            Console.WriteLine(string.Join(" ", context.Args));
            return Task.CompletedTask;
        }
    }
}