using Microsoft.Extensions.DependencyInjection;
using Qweree.ConsoleApplication.Commands;
using Qweree.ConsoleApplication.Commands.Context;
using Qweree.ConsoleApplication.Infrastructure.Commands;
using Qweree.ConsoleApplication.Infrastructure.RunContext;
using Qweree.ConsoleHost;
using Qweree.ConsoleHost.Extensions;

namespace Qweree.ConsoleApplication
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<CommandExecutorMiddleware>();
            services.AddSingleton(_ =>
            {
                var context = ContextFactory.GuessContext();
                return context;
            });
            services.AddSingleton<ICommand, RootCommand>();
            services.AddSingleton<ICommand, ContextInitCommand>();
            services.AddSingleton<ICommand, ContextReadCommand>();
        }

        public static void Configure(ConsoleApplicationBuilder app)
        {
            app.UseMiddleware<CommandExecutorMiddleware>();
        }
    }
}