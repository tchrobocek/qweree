using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Qweree.CommandLine.AspNet
{
    public delegate Task RequestDelegate(ConsoleContext context, CancellationToken cancellationToken = new());


    public class ConsoleApplicationBuilder
    {
        private readonly List<Func<RequestDelegate?, RequestDelegate>> _delegates = new();

        public ConsoleApplicationBuilder(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

        public void Use(Func<RequestDelegate?, RequestDelegate> func)
        {
            _delegates.Add(func);
        }

        public RequestDelegate Build()
        {
            return Application;
        }

        private async Task Application(ConsoleContext context, CancellationToken cancellationToken = new())
        {
            RequestDelegate? next = null;
            for (var i = _delegates.Count - 1; i >= 0; i--)
            {
                var func = _delegates[i];
                next = func(next);
            }

            if (next != null)
            {
                await next(context, cancellationToken);
            }
        }
    }
}