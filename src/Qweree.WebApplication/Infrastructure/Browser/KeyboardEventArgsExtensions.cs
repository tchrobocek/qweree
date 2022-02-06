using Microsoft.AspNetCore.Components.Web;

namespace Qweree.WebApplication.Infrastructure.Browser;

public static class KeyboardEventArgsExtensions
{
    public static bool IsEnter(this KeyboardEventArgs @this)
    {
        return @this.Code is "Enter" or "NumpadEnter";
    }
}