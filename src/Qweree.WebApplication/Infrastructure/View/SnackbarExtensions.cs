using MudBlazor;
using Qweree.Sdk;

namespace Qweree.WebApplication.Infrastructure.View;

public static class SnackbarExtensions
{
    public static void AddErrors(this ISnackbar @this, ErrorResponse response)
    {
        if (response.Errors is null)
            return;

        foreach (var error in response.Errors)
        {
            if (string.IsNullOrWhiteSpace(error.Message))
                continue;

            @this.Add(error.Message, Severity.Error);
        }
    }
}