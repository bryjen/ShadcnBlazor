using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ShadcnBlazor.Components.Shared;
using ShadcnBlazor.Components.Shared.Models.Accessibility;
using ShadcnBlazor.Components.Shared.Models.Enums;

namespace ShadcnBlazor.Components.Button;

/// <summary>
/// A clickable button with configurable variant, size, and type.
/// </summary>
public partial class Button : ShadcnComponentBase
{
    /// <summary>
    /// The content rendered inside the button.
    /// </summary>
    [Parameter]
    [Category("Content")]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The visual style variant of the button.
    /// </summary>
    [Parameter]
    [Category("Appearance")]
    public Variant Variant { get; set; } = Variant.Default;

    /// <summary>
    /// The size of the button.
    /// </summary>
    [Parameter]
    [Category("Appearance")]
    public Size Size { get; set; } = Size.Md;

    /// <summary>
    /// The button state. When Loading, the button is disabled with aria-busy. When Disabled, the button is disabled. When null, normal.
    /// </summary>
    [Parameter]
    [Category("Behavior")]
    public ButtonState? State { get; set; }

    /// <summary>
    /// The HTML button type (button, submit, or reset).
    /// </summary>
    [Parameter]
    [Category("Behavior")]
    public ButtonType Type { get; set; } = ButtonType.Button;

    /// <summary>
    /// Callback fired when the button is clicked.
    /// </summary>
    [Parameter]
    [Category("Behavior")]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    [CascadingParameter]
    private PopoverTriggerContext? PopoverTriggerContext { get; set; }

    [CascadingParameter]
    private FormValidationContext? FormValidationContext { get; set; }

    private bool IsDisabled => State is ButtonState.Loading or ButtonState.Disabled;

    private IReadOnlyDictionary<string, object>? GetAttributes()
    {
        var hasContext = PopoverTriggerContext is not null || FormValidationContext is not null || State is not null;
        if (!hasContext)
            return AdditionalAttributes;

        var merged = new Dictionary<string, object>(StringComparer.Ordinal);

        if (State is ButtonState.Loading)
            merged["aria-busy"] = true;
        if (State is ButtonState.Disabled)
            merged["aria-disabled"] = true;

        if (PopoverTriggerContext is not null)
        {
            merged["aria-expanded"] = PopoverTriggerContext.Open;
            merged["aria-haspopup"] = PopoverTriggerContext.AriaHasPopup;
            if (!string.IsNullOrEmpty(PopoverTriggerContext.PopoverId))
                merged["aria-controls"] = PopoverTriggerContext.PopoverId;
        }

        if (FormValidationContext is not null)
        {
            merged["aria-invalid"] = FormValidationContext.Invalid;
            if (!string.IsNullOrEmpty(FormValidationContext.ErrorMessageId))
                merged["aria-errormessage"] = FormValidationContext.ErrorMessageId;
        }

        if (AdditionalAttributes is not null)
            foreach (var kv in AdditionalAttributes)
                merged[kv.Key] = kv.Value;

        return merged;
    }

    private string GetClass()
    {
        return ButtonStyles.Build(base.MergeCss, Variant, Size, State, Class);
    }
}