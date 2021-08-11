using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Qweree.WebApplication.Web;

namespace Qweree.WebApplication
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            var startup = new Startup(builder.HostEnvironment);
            startup.ConfigureServices(builder.Services);

            await builder.Build().RunAsync();
        }
    }
}