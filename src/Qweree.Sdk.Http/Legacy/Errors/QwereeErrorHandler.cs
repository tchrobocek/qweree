using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Sdk.Http.Exceptions;
using Qweree.Utils;

namespace Qweree.Sdk.Http.Legacy.Errors
{
    public class QwereeErrorHandler : IErrorHandler
    {
        public async Task HandleErrorResponseAsync(HttpResponseMessage response,
            CancellationToken cancellationToken = new())
        {
            ErrorResponseDto errorResponse = new();
            try
            {
                errorResponse = await response.Content.ReadAsObjectAsync<ErrorResponseDto>(cancellationToken) ??
                                errorResponse;
            }
            catch (Exception)
            {
                // ignored
            }

            if ((int) response.StatusCode >= 400 && (int) response.StatusCode < 500)
                throw new ClientErrorException((int) response.StatusCode, errorResponse!);

            throw new ServerErrorException((int) response.StatusCode, errorResponse!);
        }
    }
}