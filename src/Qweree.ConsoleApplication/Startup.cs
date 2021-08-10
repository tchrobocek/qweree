using Microsoft.Extensions.DependencyInjection;
using Qweree.ConsoleApplication.Commands;
using Qweree.ConsoleApplication.Infrastructure.Commands;
using Qweree.ConsoleHost;
using Qweree.ConsoleHost.Extensions;

namespace Qweree.ConsoleApplication
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ICommand, HelloWorldCommand>();
            services.AddSingleton<CommandExecutorMiddleware>();
        }

        public static void Configure(ConsoleApplicationBuilder app)
        {
            app.UseMiddleware<CommandExecutorMiddleware>();
        }
    }
}