using System;
using System.Linq;
using Qweree.AspNet.Application;
using Qweree.Validator;

namespace Qweree.AspNet.Validations;

public static class ValidationResultExtensions
{
    public static Response ToErrorResponse(this ValidationResult @this)
    {
        if (@this.HasSucceeded)
            throw new ArgumentException("Result is successful.");

        return Response.Fail(@this.Errors.Select(e => $"{e.Message}."));
    }

    public static Response<TResponseType> ToErrorResponse<TResponseType>(this ValidationResult @this)
        where TResponseType : class
    {
        if (@this.HasSucceeded)
            throw new ArgumentException("Result is successful.");

        return Response.Fail<TResponseType>(@this.Errors.Select(e => $"{e.Message}."));
    }
}