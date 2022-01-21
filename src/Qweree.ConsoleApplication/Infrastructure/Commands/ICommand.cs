using System.Threading;
using System.Threading.Tasks;
using Qweree.Commands;

namespace Qweree.ConsoleApplication.Infrastructure.Commands;

public interface ICommand
{
    string CommandPath { get; }
    Task<int> ExecuteAsync(OptionsBag optionsBag, CancellationToken cancellationToken = new());
}