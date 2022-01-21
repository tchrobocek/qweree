using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Sdk.Http.Exceptions;
using Qweree.Utils;

namespace Qweree.Sdk.Http;

public class ApiResponse : IDisposable
{
    public static ApiResponse CreateApiResponse(HttpResponseMessage message)
    {
        return new ApiResponse(message);
    }
    public static ApiResponse<TPayloadType> CreateApiResponse<TPayloadType>(HttpResponseMessage message) where TPayloadType : class
    {
        return new ApiResponse<TPayloadType>(message);
    }


    public ApiResponse(HttpResponseMessage responseMessage)
    {
        ResponseMessage = responseMessage;
    }

    protected HttpResponseMessage ResponseMessage { get; }
    public bool IsSuccessful => ResponseMessage.IsSuccessStatusCode;
    public HttpStatusCode StatusCode => ResponseMessage.StatusCode;
    public HttpResponseHeaders ResponseHeaders => ResponseMessage.Headers;
    public HttpContentHeaders ContentHeaders => ResponseMessage.Content.Headers;

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

    public void Dispose()
    {
        ResponseMessage.Dispose();
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

public class PaginationApiResponse<TPayloadType> : ApiResponse<IEnumerable<TPayloadType>> where TPayloadType : class
{
    public PaginationApiResponse(HttpResponseMessage responseMessage) : base(responseMessage)
    {
    }

    public int DocumentCount
    {
        get
        {
            ResponseHeaders.TryGetValues("q-document-count", out var documentCounts);
            int.TryParse(documentCounts?.FirstOrDefault(), out var documentCount);

            return documentCount;
        }
    }
}