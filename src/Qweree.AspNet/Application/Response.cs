using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Qweree.AspNet.Application
{
    public enum ResponseStatus
    {
        Ok,
        Fail
    }

    public class Response
    {
        public static Response Ok()
        {
            return new Response(ResponseStatus.Ok, null, System.Array.Empty<Error>());
        }

        public static Response Ok(object payload)
        {
            return new Response(ResponseStatus.Ok, payload, System.Array.Empty<Error>());
        }

        public static Response<TPayloadType> Ok<TPayloadType>(TPayloadType payload) where TPayloadType : class
        {
            return new Response<TPayloadType>(ResponseStatus.Ok, payload, System.Array.Empty<Error>());
        }

        public static CollectionResponse<TPayloadType> Ok<TPayloadType>(IEnumerable<TPayloadType> payload) where TPayloadType : class
        {
            return new CollectionResponse<TPayloadType>(ResponseStatus.Ok, payload, System.Array.Empty<Error>());
        }

        public static Response Fail(IEnumerable<Error> errors)
        {
            return new Response(ResponseStatus.Fail, null, errors);
        }

        public static Response Fail(IEnumerable<string> errors)
        {
            return new Response(ResponseStatus.Fail, null, errors.Select(e => new Error(e)));
        }

        public static Response Fail(params Error[] errors)
        {
            return new Response(ResponseStatus.Fail, null, errors);
        }

        public static Response Fail(params string[] errors)
        {
            return new Response(ResponseStatus.Fail, null, errors.Select(e => new Error(e)));
        }

        public static Response<TPayloadType> Fail<TPayloadType>(IEnumerable<Error> errors) where TPayloadType : class
        {
            return new Response<TPayloadType>(ResponseStatus.Fail, null, errors);
        }

        public static Response<TPayloadType> Fail<TPayloadType>(IEnumerable<string> errors) where TPayloadType : class
        {
            return new Response<TPayloadType>(ResponseStatus.Fail, null, errors.Select(e => new Error(e)));
        }

        public static Response<TPayloadType> Fail<TPayloadType>(params Error[] errors) where TPayloadType : class
        {
            return new Response<TPayloadType>(ResponseStatus.Fail, null, errors);
        }

        public static Response<TPayloadType> Fail<TPayloadType>(params string[] errors) where TPayloadType : class
        {
            return new Response<TPayloadType>(ResponseStatus.Fail, null, errors.Select(e => new Error(e)));
        }

        public static CollectionResponse<TPayloadType> FailCollection<TPayloadType>(params string[] errors) where TPayloadType : class
        {
            return new CollectionResponse<TPayloadType>(ResponseStatus.Fail, null, errors.Select(e => new Error(e)));
        }

        public Response(ResponseStatus status, object? payload, IEnumerable<Error> errors)
        {
            Status = status;
            Payload = payload;
            Errors = errors.ToImmutableArray();
        }

        public ResponseStatus Status { get; }
        public object? Payload { get; }
        public IEnumerable<Error> Errors { get; }
    }

    public class Response<TPayloadType> : Response where TPayloadType : class
    {
        public Response(ResponseStatus status, TPayloadType? payload, IEnumerable<Error> errors) : base(status, payload, errors)
        {
            Payload = payload;
        }

        public new TPayloadType? Payload { get; }
    }

    public class CollectionResponse<TPayloadType> : Response<IEnumerable<TPayloadType>> where TPayloadType : class
    {
        public CollectionResponse(ResponseStatus status, IEnumerable<TPayloadType>? payload, IEnumerable<Error> errors) : base(status, payload?.ToImmutableArray(), errors)
        {
        }
    }

    public class Error
    {
        public Error(string message, int? code = null)
        {
            Message = message;
            Code = code;
        }

        public string Message { get; }
        public int? Code { get; }
    }
}