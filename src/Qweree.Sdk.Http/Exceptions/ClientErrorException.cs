using System;
using System.Collections.Immutable;
using System.Linq;

namespace Qweree.Sdk.Http.Exceptions
{
    public class ClientErrorException : HttpException
    {
        public ClientErrorException(int responseCode, ErrorResponseDto response) : this(
            $"{responseCode} - {string.Join(", ", response.Errors?.Select(e => e.Message) ?? ImmutableArray<string?>.Empty)}",
            responseCode, response)
        {
        }

        public ClientErrorException(string? message, int responseCode, ErrorResponseDto response) : base(message,
            responseCode, response)
        {
        }

        public ClientErrorException(string? message, Exception? innerException, int responseCode,
            ErrorResponseDto response) : base(message, innerException, responseCode, response)
        {
        }
    }
}