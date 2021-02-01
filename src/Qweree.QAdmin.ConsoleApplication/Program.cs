using System.Threading.Tasks;
using Qweree.CommandLine.AspNet;
using Qweree.CommandLine.AspNet.Extensions;

namespace Qweree.ConsoleApplication
{
    internal class Program
    {
        private static async Task<int> Main(string[] args)
        {
            var host = new ConsoleHost
            {
                ConfigureServicesAction = Startup.ConfigureServices,
                ConfigureAction = Startup.Configure
            };

            return await host.UseConsoleListener(new ConsoleListener(args))
                .RunAsync(args);
        }
    }
}