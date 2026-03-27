namespace ShadcnBlazor.Components.Popover.Models;

/// <summary>
/// Types of interaction outside a popover.
/// </summary>
public enum InteractOutsideEventType
{
    /// <summary>Pointer down event outside the popover.</summary>
    PointerDown,
    
    /// <summary>Focus event outside the popover.</summary>
    Focus
}

/// <summary>
/// Arguments for the OnInteractOutside event.
/// </summary>
public class InteractOutsideEventArgs
{
    /// <summary>
    /// The type of interaction that occurred.
    /// </summary>
    public InteractOutsideEventType EventType { get; init; }

    /// <summary>
    /// Whether the interaction has been handled. If true, the default close behavior is suppressed.
    /// </summary>
    public bool Handled { get; set; }
}
