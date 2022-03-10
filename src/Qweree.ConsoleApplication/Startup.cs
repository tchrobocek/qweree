using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Qweree.Authentication.Sdk.Http;
using Qweree.ConsoleApplication.Commands;
using Qweree.ConsoleApplication.Commands.Context;
using Qweree.ConsoleApplication.Commands.Picc;
using Qweree.ConsoleApplication.Infrastructure;
using Qweree.ConsoleApplication.Infrastructure.Authentication;
using Qweree.ConsoleApplication.Infrastructure.Commands;
using Qweree.ConsoleApplication.Infrastructure.ErrorHandling;
using Qweree.ConsoleApplication.Infrastructure.RunContext;
using Qweree.ConsoleHost;
using Qweree.ConsoleHost.Extensions;

namespace Qweree.ConsoleApplication;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ErrorHandlingMiddleware>();
        services.AddSingleton<CommandExecutorMiddleware>();
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
        services.AddSingleton<HttpMessageHandlerFactory>();
        services.AddSingleton<ICommand, RootCommand>();
        services.AddSingleton<ICommand, ContextInitCommand>();
        services.AddSingleton<ICommand, ContextReadCommand>();
        services.AddSingleton<ICommand, ContextLocationCommand>();
        services.AddSingleton<ICommand, LoginCommand>();
        services.AddSingleton<ICommand, PiccStashCommand>();
    }

    public static void Configure(ConsoleApplicationBuilder app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseMiddleware<CommandExecutorMiddleware>();
    }
}