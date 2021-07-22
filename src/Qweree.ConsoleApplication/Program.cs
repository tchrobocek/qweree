using System.Threading.Tasks;
using Qweree.ConsoleHost;

namespace Qweree.ConsoleApplication
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var host = new Host
            {
                ConfigureAction = Startup.Configure,
                ConfigureServicesAction = Startup.ConfigureServices
            };

            return await host.Build()
                .RunAsync(args);
        }
    }
}