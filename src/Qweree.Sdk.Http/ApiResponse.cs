using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Sdk.Http.Exceptions;
using Qweree.Utils;

namespace Qweree.Sdk.Http
{
    public class ApiResponse
    {
        public static ApiResponse CreateApiResponse(HttpResponseMessage message)
        {
            return new (message);
        }
        public static ApiResponse<TPayloadType> CreateApiResponse<TPayloadType>(HttpResponseMessage message) where TPayloadType : class
        {
            return new (message);
        }


        public ApiResponse(HttpResponseMessage responseMessage)
        {
            ResponseMessage = responseMessage;
        }

        protected HttpResponseMessage ResponseMessage { get; }
        public bool IsSuccessful => ResponseMessage.IsSuccessStatusCode;
        public HttpStatusCode StatusCode => ResponseMessage.StatusCode;

        public void EnsureSuccessStatusCode()
        {
            var statusCodeInt = (int) ResponseMessage.StatusCode;
            if (statusCodeInt < 400)
                return;

            if (statusCodeInt < 500)
                throw new ClientErrorErrorException(this, "Server did not return successful status code.");

            if (statusCodeInt < 600)
                throw new ServerErrorErrorException(this, "Server did not return successful status code.");

            throw new HttpErrorException(this, "Server did not return successful status code.");
        }

        public async Task<Stream> ReadPayloadAsStreamAsync(CancellationToken cancellationToken = new())
        {
            return await ResponseMessage.Content.ReadAsStreamAsync(cancellationToken);
        }

        public async Task<byte[]> ReadPayloadAsByteArrayAsync(CancellationToken cancellationToken = new())
        {
            return await ResponseMessage.Content.ReadAsByteArrayAsync(cancellationToken);
        }

        public async Task<ErrorResponseDto> ReadErrorsAsync(CancellationToken cancellationToken = new())
        {
            if (IsSuccessful)
            {
                return new ErrorResponseDto();
            }

            try
            {
                return await ResponseMessage.Content.ReadAsObjectAsync<ErrorResponseDto>(cancellationToken)
                       ?? new ErrorResponseDto();
            }
            catch (Exception)
            {
                return new ErrorResponseDto();
            }
        }
    }

    public class ApiResponse<TPayloadType> : ApiResponse where TPayloadType : class
    {
        public ApiResponse(HttpResponseMessage responseMessage) : base(responseMessage)
        {
        }

        public async Task<TPayloadType?> ReadPayloadAsync(CancellationToken cancellationToken = new())
        {
            return await ResponseMessage.Content.ReadAsObjectAsync<TPayloadType>(cancellationToken);
        }

        public async Task<TPayloadType?> ReadPayloadAsync(JsonSerializerOptions options, CancellationToken cancellationToken = new())
        {
            return await ResponseMessage.Content.ReadAsObjectAsync<TPayloadType>(options, cancellationToken);
        }
    }
}