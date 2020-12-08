using System;

namespace Qweree.Sdk.Http.Errors.Exceptions
{
    public class ClientErrorException : HttpException
    {
        public ClientErrorException(int responseCode, ErrorResponseDto response) : base(responseCode, response)
        {
        }

        public ClientErrorException(string? message, int responseCode, ErrorResponseDto response) : base(message, responseCode, response)
        {
        }

        public ClientErrorException(string? message, Exception? innerException, int responseCode, ErrorResponseDto response) : base(message, innerException, responseCode, response)
        {
        }
    }
}