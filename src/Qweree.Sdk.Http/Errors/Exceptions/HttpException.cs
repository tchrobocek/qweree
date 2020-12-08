using System;

namespace Qweree.Sdk.Http.Errors.Exceptions
{
    public class HttpException : Exception
    {
        public HttpException(int responseCode, ErrorResponseDto response)
        {
            ResponseCode = responseCode;
            Response = response;
        }

        public HttpException(string? message, int responseCode, ErrorResponseDto response) : base(message)
        {
            ResponseCode = responseCode;
            Response = response;
        }

        public HttpException(string? message, Exception? innerException, int responseCode, ErrorResponseDto response) : base(message, innerException)
        {
            ResponseCode = responseCode;
            Response = response;
        }

        public int ResponseCode { get; }
        public ErrorResponseDto Response { get; }
    }
}