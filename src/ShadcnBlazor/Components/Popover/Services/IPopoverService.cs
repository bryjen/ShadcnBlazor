using Microsoft.JSInterop;

namespace ShadcnBlazor.Components.Popover.Services;

/// <summary>
/// Service for popover JavaScript interop (positioning, outside-click, etc.).
/// </summary>
public interface IPopoverService
{
    /// <summary>
    /// Initializes the popover JavaScript module with the given options.
    /// </summary>
    Task InitializeAsync(string containerClass, int flipMargin, int overflowPadding, int baseZIndex);

    /// <summary>
    /// Sets the debounce delay for repositioning.
    /// </summary>
    Task SetRepositionDebounceAsync(int debounceMilliseconds);

    /// <summary>
    /// Enables close-on-outside-click for a popover.
    /// </summary>
    Task EnableOutsideClickCloseAsync(string anchorId, string popoverId, DotNetObjectReference<Popover> callbackReference);

    /// <summary>
    /// Disables close-on-outside-click for a popover.
    /// </summary>
    Task DisableOutsideClickCloseAsync(string popoverId);

    /// <summary>
    /// Connects an anchor to its popover for positioning.
    /// </summary>
    Task ConnectAsync(string anchorId, string popoverId);

    /// <summary>
    /// Disconnects a popover from its anchor.
    /// </summary>
    Task DisconnectAsync(string popoverId);

    /// <summary>
    /// Disposes the JavaScript interop resources.
    /// </summary>
    ValueTask DisposeAsync();
}
