using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Qweree.ConsoleApplication.Commands;
using Qweree.ConsoleApplication.Commands.Context;
using Qweree.ConsoleApplication.Commands.Picc;
using Qweree.ConsoleApplication.Infrastructure.Authentication;
using Qweree.ConsoleApplication.Infrastructure.Commands;
using Qweree.ConsoleApplication.Infrastructure.RunContext;
using Qweree.ConsoleHost;
using Qweree.ConsoleHost.Extensions;
using Qweree.Sdk.Http.HttpClient;

namespace Qweree.ConsoleApplication
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<CommandExecutorMiddleware>();
            services.AddSingleton<RefreshTokenMiddleware>();
            services.AddSingleton<ITokenStorage, MemoryTokenStorage>();
            services.AddSingleton(p =>
            {
                var context = ContextFactory.GuessContext(p.GetRequiredService<ITokenStorage>());
                return context;
            });
            services.AddSingleton<HttpMessageHandler, HttpClientHandler>();
            services.AddSingleton<OAuth2ClientFactory>();
            services.AddSingleton<PiccClientFactory>();
            services.AddSingleton<AuthenticationService>();
            services.AddSingleton<ICommand, RootCommand>();
            services.AddSingleton<ICommand, ContextInitCommand>();
            services.AddSingleton<ICommand, ContextReadCommand>();
            services.AddSingleton<ICommand, ContextLocationCommand>();
            services.AddSingleton<ICommand, LoginCommand>();
            services.AddSingleton<ICommand, PiccStashCommand>();
        }

        public static void Configure(ConsoleApplicationBuilder app)
        {
            app.UseMiddleware<RefreshTokenMiddleware>();
            app.UseMiddleware<CommandExecutorMiddleware>();
        }
    }
}