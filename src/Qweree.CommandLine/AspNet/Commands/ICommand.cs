using System.Threading;
using System.Threading.Tasks;
using Qweree.CommandLine.Commands;

namespace Qweree.CommandLine.AspNet.Commands
{
    public interface ICommand
    {
        void Configure(ConfigurationBuilder config);
        Task<int> ExecuteAsync(CommandRequest request, CancellationToken cancellationToken = new());
    }
}