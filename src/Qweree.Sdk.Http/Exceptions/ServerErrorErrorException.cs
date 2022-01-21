using System;
using System.Runtime.Serialization;

namespace Qweree.Sdk.Http.Exceptions;

public class ServerErrorErrorException : HttpErrorException
{
    public ServerErrorErrorException(ApiResponse apiResponse) : base(apiResponse)
    {
    }

    protected ServerErrorErrorException(ApiResponse apiResponse, SerializationInfo info, StreamingContext context) : base(apiResponse, info, context)
    {
    }

    public ServerErrorErrorException(ApiResponse apiResponse, string? message) : base(apiResponse, message)
    {
    }

    public ServerErrorErrorException(ApiResponse apiResponse, string? message, Exception? innerException) : base(apiResponse, message, innerException)
    {
    }
}