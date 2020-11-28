using System;
using Qweree.Sdk;

namespace Qweree.Authentication.Sdk.Exceptions
{
    public class ServerErrorException : HttpException
    {
        public ServerErrorException(int responseCode, ErrorResponseDto response) : base(responseCode, response)
        {
        }

        public ServerErrorException(string? message, int responseCode, ErrorResponseDto response) : base(message, responseCode, response)
        {
        }

        public ServerErrorException(string? message, Exception? innerException, int responseCode, ErrorResponseDto response) : base(message, innerException, responseCode, response)
        {
        }
    }
}