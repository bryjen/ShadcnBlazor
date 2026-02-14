namespace ShadcnBlazor.Components.Popover.Services;

/// <summary>
/// Implementation of <see cref="IPopoverRegistry"/> that tracks the current PopoverProvider.
/// </summary>
public sealed class PopoverRegistry : IPopoverRegistry
{
    /// <inheritdoc />
    public PopoverProvider? CurrentProvider { get; private set; }

    /// <inheritdoc />
    public void SetProvider(PopoverProvider provider)
    {
        CurrentProvider = provider;
    }

    /// <inheritdoc />
    public void ClearProvider(PopoverProvider provider)
    {
        if (ReferenceEquals(CurrentProvider, provider))
        {
            CurrentProvider = null;
        }
    }
}
