using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Sdk.Http.Errors.Exceptions;
using Qweree.Utils;

namespace Qweree.Sdk.Http.Errors
{
    public class QwereeErrorHandler : IErrorHandler
    {
        public async Task HandleErrorResponse(HttpResponseMessage response, CancellationToken cancellationToken = new CancellationToken())
        {
            var errorResponse = await response.Content.ReadAsObjectAsync<ErrorResponseDto>(cancellationToken);

            if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
                throw new ClientErrorException((int)response.StatusCode, errorResponse!);

            throw new ServerErrorException((int)response.StatusCode, errorResponse!);
        }
    }
}