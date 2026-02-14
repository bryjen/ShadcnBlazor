namespace ShadcnBlazor.Components.Popover.Services;

/// <summary>
/// Registry for the current PopoverProvider instance.
/// </summary>
public interface IPopoverRegistry
{
    /// <summary>
    /// The currently active PopoverProvider, or null if none is registered.
    /// </summary>
    PopoverProvider? CurrentProvider { get; }

    /// <summary>
    /// Registers a PopoverProvider as the current provider.
    /// </summary>
    /// <param name="provider">The provider to register.</param>
    void SetProvider(PopoverProvider provider);

    /// <summary>
    /// Clears the provider when it is disposed.
    /// </summary>
    /// <param name="provider">The provider being disposed.</param>
    void ClearProvider(PopoverProvider provider);
}
