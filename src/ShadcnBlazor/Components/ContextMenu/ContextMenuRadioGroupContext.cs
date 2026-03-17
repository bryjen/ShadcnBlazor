namespace ShadcnBlazor.Components.ContextMenu;

/// <summary>
/// Context for ContextMenuRadioGroup and its radio items.
/// </summary>
public class ContextMenuRadioGroupContext
{
    /// <summary>
    /// The currently selected value.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Callback invoked when a radio item is selected.
    /// </summary>
    public Func<string, Task>? OnValueChanged { get; set; }
}
