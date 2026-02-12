namespace ShadcnBlazor.Components.Popover.Services;

public sealed class PopoverRegistry : IPopoverRegistry
{
    public PopoverProvider? CurrentProvider { get; private set; }

    public void SetProvider(PopoverProvider provider)
    {
        CurrentProvider = provider;
    }

    public void ClearProvider(PopoverProvider provider)
    {
        if (ReferenceEquals(CurrentProvider, provider))
        {
            CurrentProvider = null;
        }
    }
}
