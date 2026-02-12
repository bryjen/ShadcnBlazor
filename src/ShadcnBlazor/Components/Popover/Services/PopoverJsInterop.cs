using Microsoft.JSInterop;

namespace ShadcnBlazor.Components.Popover.Services;

internal class PopoverJsInterop
{
    private readonly IJSRuntime _jsRuntime;

    public PopoverJsInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public ValueTask InitializeAsync(
        string containerClass,
        int flipMargin,
        int overflowPadding,
        int baseZIndex,
        CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeVoidAsync(
            "popoverManager.initialize",
            cancellationToken,
            containerClass,
            flipMargin,
            overflowPadding,
            baseZIndex);
    }

    public ValueTask SetRepositionDebounceAsync(int debounceMilliseconds, CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeVoidAsync(
            "popoverManager.setRepositionDebounce",
            cancellationToken,
            debounceMilliseconds);
    }

    public ValueTask EnableOutsideClickCloseAsync(string anchorId, string popoverId, DotNetObjectReference<Popover> callbackReference, CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeVoidAsync(
            "popoverManager.enableOutsideClickClose",
            cancellationToken,
            anchorId,
            popoverId,
            callbackReference);
    }

    public ValueTask DisableOutsideClickCloseAsync(string popoverId, CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeVoidAsync(
            "popoverManager.disableOutsideClickClose",
            cancellationToken,
            popoverId);
    }

    public ValueTask ConnectAsync(
        string anchorId,
        string popoverId,
        CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeVoidAsync(
            "popoverManager.connect",
            cancellationToken,
            anchorId,
            popoverId);
    }

    public ValueTask DisconnectAsync(string popoverId, CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeVoidAsync(
            "popoverManager.disconnect",
            cancellationToken,
            popoverId);
    }

    public ValueTask DisposeAsync(CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeVoidAsync("popoverManager.dispose", cancellationToken);
    }
}
