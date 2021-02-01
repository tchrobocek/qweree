using System.Threading;
using System.Threading.Tasks;
using Qweree.CommandLine.CommandRouter;

namespace Qweree.CommandLine.AspNet.CommandRouter
{
    public interface ICommand
    {
        void Configure(ConfigurationBuilder config);
        Task<int> ExecuteAsync(CommandRequest request, CancellationToken cancellationToken = new());
    }
}