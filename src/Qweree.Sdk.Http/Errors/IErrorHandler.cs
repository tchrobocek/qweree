using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Qweree.Sdk.Http.Errors
{
    public interface IErrorHandler
    {
        Task HandleErrorResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken = new());
    }
}