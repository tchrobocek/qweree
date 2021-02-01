#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Qweree.CommandLine.AspNet;
using Qweree.CommandLine.AspNet.CommandRouter;
using Qweree.CommandLine.AspNet.Extensions;
using Qweree.ConsoleApplication.Commands;
using Qweree.ConsoleApplication.Infrastructure.ErrorHandling;

namespace Qweree.ConsoleApplication
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddCommandRouter();
            services.AddScoped<ICommand, HelloWorldCommand>();
            services.AddSingleton<ErrorHandlerMiddleware>();
        }

        public static void Configure(ConsoleApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseCommandRouter();
        }
    }
}