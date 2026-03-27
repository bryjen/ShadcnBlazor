namespace ShadcnBlazor.Components.FocusTrap.Models;

public class FocusOptions
{
    public string? InitialFocus { get; set; }
    public string? FallbackFocus { get; set; }
    public bool EscapeDeactivates { get; set; } = true;
    public bool ClickOutsideDeactivates { get; set; } = false;
    public bool ReturnFocusOnDeactivate { get; set; } = true;
    public bool AllowOutsideClick { get; set; } = true;
}