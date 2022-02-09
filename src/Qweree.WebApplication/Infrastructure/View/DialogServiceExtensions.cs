using System.Threading.Tasks;
using MudBlazor;
using Qweree.WebApplication.Web.Components.Dialog;

namespace Qweree.WebApplication.Infrastructure.View;

public static class DialogServiceExtensions
{

    public static async Task AlertAsync(this DialogService @this, string message)
    {
        await @this.AlertAsync("Alert", message);
    }

    public static async Task AlertAsync(this DialogService @this, string title, string message)
    {
        await @this.ShowAndWaitAsync<AlertDialogComponent>(title, new DialogParameters{{"text", message}});
    }

    public static async Task<bool> ConfirmAsync(this DialogService @this, string message)
    {
        return await @this.ConfirmAsync("Confirm", message);
    }

    public static async Task<bool> ConfirmAsync(this DialogService @this, string title, string message)
    {
        return await @this.ShowAndWaitAsync<ConfirmDialogComponent, bool>(title, new DialogParameters{{"text", message}});
    }
}