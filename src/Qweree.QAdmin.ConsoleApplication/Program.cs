﻿using System.Threading.Tasks;
using Qweree.CommandLine.AspNet;

namespace Qweree.ConsoleApplication
{
    internal class Program
    {
        private static async Task<int> Main(string[] args)
        {
            var host = new ConsoleHost
            {
                ConfigureServicesAction = Startup.ConfigureServices,
                ConfigureAction = Startup.Configure,
                ConsoleListenerAction = Startup.Listen
            };

            return await host.RunAsync(args);
        }
    }
}