using System;
using Microsoft.Extensions.DependencyInjection;
using Qweree.ConsoleHost;

namespace Qweree.ConsoleApplication
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
        }

        public static void Configure(ConsoleApplicationBuilder app)
        {
            Console.WriteLine("Hello!");
            Console.WriteLine("Hey!");
        }
    }
}