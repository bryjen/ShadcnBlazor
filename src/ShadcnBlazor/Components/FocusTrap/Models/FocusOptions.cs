namespace ShadcnBlazor.Components.FocusTrap.Models;

/// <summary>
/// Configuration options for a focus trap.
/// </summary>
public class FocusOptions
{
    /// <summary>The element to focus when the trap is activated.</summary>
    public string? InitialFocus { get; set; }
    /// <summary>The element to focus if the initial focus element cannot be found.</summary>
    public string? FallbackFocus { get; set; }
    /// <summary>Whether pressing 'Escape' deactivates the focus trap.</summary>
    public bool EscapeDeactivates { get; set; } = true;
    /// <summary>Whether clicking outside the trap deactivates it.</summary>
    public bool ClickOutsideDeactivates { get; set; } = false;
    /// <summary>Whether focus is returned to the original element after deactivation.</summary>
    public bool ReturnFocusOnDeactivate { get; set; } = true;
    /// <summary>Whether clicks outside the trap are allowed while active.</summary>
    public bool AllowOutsideClick { get; set; } = true;
}