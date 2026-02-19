using Microsoft.JSInterop;
using ShadcnBlazor.Components.Shared;
using ShadcnBlazor.Components.Shared.Models.Options;

namespace ShadcnBlazor.Docs.Components.Samples.Command.Inline;

// Logic for key-interceptor specific logic (properties & callback methods).
// Logic for executing certain operations (ex. registration & disposing) is delegated to main component.

public partial class CommandInline : ShadcnComponentBase
{
    private readonly string _elementId = "command-inline-" + Guid.NewGuid().ToString("N")[..8];
    private readonly string _contentId = "command-inline-content-" + Guid.NewGuid().ToString("N")[..8];
    private DotNetObjectReference<CommandInline>? _dotNetRef;
    private KeyInterceptorEventArgs? _lastKeyEvent;
    private IJSObjectReference? _scrollModule;
    
    [JSInvokable]
    public async void OnKeyDown(string elementId, KeyInterceptorEventArgs args)
    {
        try
        {
            _lastKeyEvent = args with { Type = "keydown" };
            var filtered = GetFilteredCommandItems();
            if (filtered.Count == 0 || _selectedItem is null)
                return;

            var idx = filtered.Select((item, i) => (item, i)).First(x => x.item!.GetHashCode() == _selectedItem).i;

            switch (args.Key)
            {
                case "Tab" when args.ShiftKey:
                case "ArrowUp":
                    var prevIdx = (idx - 1 + filtered.Count) % filtered.Count;
                    _selectedItem = filtered[prevIdx]?.GetHashCode();
                    _pendingScrollItem = _selectedItem;
                    break;
                case "Tab":
                case "ArrowDown":
                    var nextIdx = (idx + 1) % filtered.Count;
                    _selectedItem = filtered[nextIdx]?.GetHashCode();
                    _pendingScrollItem = _selectedItem;
                    break;
                case "Enter":
                    if (_selectedItem is not null)
                        await JsRuntime.InvokeVoidAsync("eval", $"document.getElementById('{_selectedItem}')?.click()");
                    break;
            }
            await InvokeAsync(StateHasChanged);
        }
        catch
        {
            // ignored
        }
    }

    [JSInvokable]
    public void OnKeyUp(string elementId, KeyInterceptorEventArgs args)
    {
        // ignored
    }
}