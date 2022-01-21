using System;
using System.Runtime.Serialization;

namespace Qweree.Sdk.Http.Exceptions;

public class ClientErrorErrorException : HttpErrorException
{
    public ClientErrorErrorException(ApiResponse apiResponse) : base(apiResponse)
    {
    }

    protected ClientErrorErrorException(ApiResponse apiResponse, SerializationInfo info, StreamingContext context) : base(apiResponse, info, context)
    {
    }

    public ClientErrorErrorException(ApiResponse apiResponse, string? message) : base(apiResponse, message)
    {
    }

    public ClientErrorErrorException(ApiResponse apiResponse, string? message, Exception? innerException) : base(apiResponse, message, innerException)
    {
    }
}