namespace ShadcnBlazor.Components.Popover;

public interface IPopoverRegistry
{
    PopoverProvider? CurrentProvider { get; }
    void SetProvider(PopoverProvider provider);
    void ClearProvider(PopoverProvider provider);
}
