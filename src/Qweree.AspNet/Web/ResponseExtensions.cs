using System.Linq;
using Qweree.AspNet.Application;
using Qweree.Sdk;

namespace Qweree.AspNet.Web
{
    public static class ResponseExtensions
    {
        public static ErrorResponseDto ToErrorResponseDto(this Response response)
        {
            return new ErrorResponseDto
            {
                Errors = response.Errors.Select(e => new ErrorDto{Message = e.Message}).ToArray()
            };
        }
    }
}