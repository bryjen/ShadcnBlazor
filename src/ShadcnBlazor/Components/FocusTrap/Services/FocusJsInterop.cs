using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShadcnBlazor.Components.FocusTrap.Models;

namespace ShadcnBlazor.Components.FocusTrap.Services;

public class FocusJsInterop : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _module;

    public FocusJsInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    private async ValueTask<IJSObjectReference> GetModuleAsync()
    {
        return _module ??= await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/_content/ShadcnBlazor/Components/FocusTrap/FocusTrap.razor.js");
    }

    public async ValueTask<IJSObjectReference> CreateTrapAsync(ElementReference container, FocusOptions? options = null)
    {
        var module = await GetModuleAsync();
        return await module.InvokeAsync<IJSObjectReference>("createTrap", container, options ?? new FocusOptions());
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            await _module.DisposeAsync();
        }
    }
}