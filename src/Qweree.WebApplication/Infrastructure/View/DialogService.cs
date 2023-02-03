using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Qweree.WebApplication.Infrastructure.View;

public class DialogService
{
    private readonly IDialogService _dialogService;

    public DialogService(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public async Task<DialogResult> ShowAndWaitAsync<TComponentType>(string title, DialogParameters parameters) where TComponentType : ComponentBase
    {
        var instance = _dialogService.Show<TComponentType>(title, parameters);
        return await instance.Result;
    }

    public async Task<TResultType?> ShowAndWaitAsync<TComponentType, TResultType>(string title, DialogParameters parameters) where TComponentType : ComponentBase
    {
        var result = await ShowAndWaitAsync<TComponentType>(title, parameters);
        if (result.Canceled)
            return default;

        return (TResultType?)result.Data;
    }

    public async Task<DialogResult> ShowAndWaitAsync<TComponentType>(string title, DialogOptions options) where TComponentType : ComponentBase
    {
        var instance = _dialogService.Show<TComponentType>(title, options);
        return await instance.Result;
    }

    public async Task<TResultType?> ShowAndWaitAsync<TComponentType, TResultType>(string title, DialogOptions options) where TComponentType : ComponentBase
    {
        var result = await ShowAndWaitAsync<TComponentType>(title, options);
        if (result.Canceled)
            return default;

        return (TResultType?)result.Data;
    }
}