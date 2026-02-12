using Microsoft.JSInterop;

namespace ShadcnBlazor.Components.Popover.Services;

public interface IPopoverService
{
    Task InitializeAsync(string containerClass, int flipMargin, int overflowPadding, int baseZIndex);
    Task SetRepositionDebounceAsync(int debounceMilliseconds);
    Task EnableOutsideClickCloseAsync(string anchorId, string popoverId, DotNetObjectReference<Popover> callbackReference);
    Task DisableOutsideClickCloseAsync(string popoverId);
    Task ConnectAsync(string anchorId, string popoverId);
    Task DisconnectAsync(string popoverId);
    ValueTask DisposeAsync();
}
