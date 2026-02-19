using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ShadcnBlazor.Docs.Layout;

public partial class MainLayout : LayoutComponentBase, IAsyncDisposable
{
    [Inject]
    public IJSRuntime JsRuntime { get; set; } = null!;

    private IJSObjectReference? _shortcutModule;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        _shortcutModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/global-shortcut-manager.js");
        await _shortcutModule.InvokeVoidAsync("registerGlobalShortcuts");
    }

    public async ValueTask DisposeAsync()
    {
        if (_shortcutModule is null)
            return;

        try
        {
            await _shortcutModule.InvokeVoidAsync("unregisterGlobalShortcuts");
        }
        catch
        {
            // JS runtime may already be torn down.
        }

        await _shortcutModule.DisposeAsync();
        _shortcutModule = null;
    }
}
