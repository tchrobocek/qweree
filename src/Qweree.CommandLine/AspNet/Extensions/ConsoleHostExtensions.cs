namespace Qweree.CommandLine.AspNet.Extensions
{
    public static class ConsoleHostExtensions
    {
        public static ConsoleHost UseConsoleListener(this ConsoleHost host, IConsoleListener listener)
        {
            host.RunApplicationAction = async (_, next, cancellationToken) =>
                await listener.RunAsync(next, cancellationToken);

            return host;
        }
    }
}