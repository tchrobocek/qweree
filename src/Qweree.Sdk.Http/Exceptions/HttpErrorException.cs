using System;
using System.Runtime.Serialization;

namespace Qweree.Sdk.Http.Exceptions
{
    public class HttpErrorException : Exception
    {
        public HttpErrorException(ApiResponse apiResponse)
        {
            ApiResponse = apiResponse;
        }

        protected HttpErrorException(ApiResponse apiResponse, SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ApiResponse = apiResponse;
        }

        public HttpErrorException(ApiResponse apiResponse, string? message) : base(message)
        {
            ApiResponse = apiResponse;
        }

        public HttpErrorException(ApiResponse apiResponse, string? message, Exception? innerException) : base(message, innerException)
        {
            ApiResponse = apiResponse;
        }

        public ApiResponse ApiResponse { get; }
    }
}