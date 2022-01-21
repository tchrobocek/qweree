using System;
using System.Collections.Generic;
using System.Net.Mime;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Qweree.AspNet.Web.Swagger;

public class FileFromBodyOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attributes = context.MethodInfo.GetCustomAttributes(typeof(RequiresFileFromBodyAttribute), true);

        if (!(attributes is RequiresFileFromBodyAttribute[]) || attributes.Length == 0) return;

        var definition = new OpenApiMediaType
        {
            Encoding = new Dictionary<string, OpenApiEncoding>
            {
                {
                    "file", new OpenApiEncoding
                    {
                        Style = ParameterStyle.Form
                    }
                }
            },
            Schema = new OpenApiSchema
            {
                Type = "file",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    {
                        "file", new OpenApiSchema
                        {
                            Type = "file",
                            Items = new OpenApiSchema
                            {
                                Format = "binary",
                                Type = "string"
                            }
                        }
                    }
                }
            }
        };

        operation.RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                {MediaTypeNames.Application.Octet, definition},
                {MediaTypeNames.Application.Json, definition},
                {MediaTypeNames.Application.Pdf, definition},
                {MediaTypeNames.Application.Xml, definition},
                {MediaTypeNames.Application.Zip, definition},
                {MediaTypeNames.Image.Gif, definition},
                {MediaTypeNames.Image.Jpeg, definition},
                {MediaTypeNames.Image.Tiff, definition},
                {MediaTypeNames.Text.Html, definition},
                {MediaTypeNames.Text.Plain, definition},
                {MediaTypeNames.Text.Xml, definition},
                {MediaTypeNames.Text.RichText, definition}
            }
        };
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class RequiresFileFromBodyAttribute : Attribute
{
}