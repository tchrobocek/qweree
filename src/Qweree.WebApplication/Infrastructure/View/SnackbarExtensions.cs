using MudBlazor;
using Qweree.Sdk;

namespace Qweree.WebApplication.Infrastructure.View;

public static class SnackbarExtensions
{
    public static void AddErrors(this ISnackbar @this, ErrorResponseDto responseDto)
    {
        if (responseDto.Errors is null)
            return;

        foreach (var error in responseDto.Errors)
        {
            if (string.IsNullOrWhiteSpace(error.Message))
                continue;

            @this.Add(error.Message, Severity.Error);
        }
    }
}