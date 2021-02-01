#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Qweree.CommandLine.AspNet;
using Qweree.CommandLine.AspNet.CommandRouter;
using Qweree.CommandLine.AspNet.Extensions;
using Qweree.ConsoleApplication.Infrastructure.ErrorHandling;

namespace Qweree.ConsoleApplication
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddCommandRouter();
            services.AddSingleton<ErrorHandlerMiddleware>();
            services.AddSingleton<HelloWorldMiddleware>();
        }

        public static void Configure(ConsoleApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseCommandRouter();
            app.UseMiddleware<HelloWorldMiddleware>();
        }
    }

    public class HelloWorldMiddleware : IMiddleware
    {
        public Task NextAsync(ConsoleContext context, RequestDelegate? next,
            CancellationToken cancellationToken = new())
        {
            Console.WriteLine("This is error test.");
            throw new Exception("Hello world!");
        }
    }
}