namespace ShadcnBlazor.Components.Popover;

public interface IPopoverService
{
    Task InitializeAsync(string containerClass, int flipMargin, int overflowPadding, int baseZIndex);
    Task SetRepositionDebounceAsync(int debounceMilliseconds);
    Task ConnectAsync(string anchorId, string popoverId);
    Task DisconnectAsync(string popoverId);
    ValueTask DisposeAsync();
}
