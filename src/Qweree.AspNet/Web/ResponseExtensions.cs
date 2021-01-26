using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Application;
using Qweree.Sdk;

namespace Qweree.AspNet.Web
{
    public static class ResponseExtensions
    {
        public static ErrorResponseDto ToErrorResponseDto(this Response response)
        {
            return new()
            {
                Errors = response.Errors.Select(e => new ErrorDto {Message = e.Message}).ToArray()
            };
        }

        public static IActionResult ToErrorActionResult(this Response response)
        {
            var result = new ObjectResult(response.ToErrorResponseDto())
            {
                StatusCode = response.Errors.Max(e => e.Code ?? 400)
            };

            return result;
        }
    }
}