namespace ShadcnBlazor.Components.Shared.Models.Accessibility;

/// <summary>
/// Context cascaded by form components to provide validation state for ARIA attributes.
/// </summary>
public class FormValidationContext
{
    /// <summary>
    /// Whether the associated form control is in an invalid state.
    /// </summary>
    public bool Invalid { get; set; }

    /// <summary>
    /// ID of the element that displays the error message. Used for aria-errormessage.
    /// </summary>
    public string? ErrorMessageId { get; set; }
}
