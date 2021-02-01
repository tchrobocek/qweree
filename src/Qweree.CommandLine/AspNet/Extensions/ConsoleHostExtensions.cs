namespace Qweree.CommandLine.AspNet.Extensions
{
    public static class ConsoleHostExtensions
    {
        public static ConsoleHost UseConsoleListener(this ConsoleHost host, IConsoleListener listener)
        {
            host.RunApplicationAction = async (_, buildAppFunc, cancellationToken) =>
            {
                return await listener.RunAsync(buildAppFunc, cancellationToken);
            };

            return host;
        }
    }
}