namespace ShadcnBlazor.Components.Popover.Services;

public interface IPopoverRegistry
{
    PopoverProvider? CurrentProvider { get; }
    void SetProvider(PopoverProvider provider);
    void ClearProvider(PopoverProvider provider);
}
