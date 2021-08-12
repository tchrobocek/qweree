using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Qweree.Sdk.Http.Legacy.Errors
{
    public interface IErrorHandler
    {
        Task HandleErrorResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken = new());
    }
}