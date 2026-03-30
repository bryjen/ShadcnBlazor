using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using ShadcnBlazor.Components.Field;
using ShadcnBlazor.Components.Shared;
using TailwindMerge;

namespace ShadcnBlazor.Components.Textarea;

/// <summary>
/// Multi-line text input for longer form content.
/// </summary>
public partial class Textarea : ShadcnComponentBase
{
    /// <summary>
    /// The current value of the textarea.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Data)]
    public string? Value { get; set; }

    /// <summary>
    /// Callback invoked when the value changes.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// The number of visible text rows.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Appearance)]
    public int Rows { get; set; } = 4;

    /// <summary>
    /// Placeholder text when the value is empty.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Common)]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Whether the textarea is disabled.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public bool Disabled { get; set; }

    [CascadingParameter]
    private EditContext? EditContext { get; set; }

    [CascadingParameter]
    private FieldContext? FieldContext { get; set; }

    private bool IsInvalid
    {
        get
        {
            if (EditContext is null || FieldContext?.For is null)
                return false;

            var fieldId = FieldIdentifier.Create(FieldContext.For);
            return EditContext.GetValidationMessages(fieldId).Any();
        }
    }

    private IReadOnlyDictionary<string, object>? GetAttributes()
    {
        if (!IsInvalid)
            return AdditionalAttributes;

        var merged = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            ["aria-invalid"] = "true"
        };

        if (AdditionalAttributes is not null)
            foreach (var kv in AdditionalAttributes)
                merged[kv.Key] = kv.Value;

        return merged;
    }

    /// <summary>
    /// Returns the CSS classes for the textarea.
    /// </summary>
    public string GetClass()
    {
        var baseClasses = "border-input placeholder:text-muted-foreground focus-visible:border-ring focus-visible:ring-ring/50 aria-invalid:ring-destructive/20 dark:aria-invalid:ring-destructive/40 aria-invalid:border-destructive dark:bg-input/30 flex min-h-16 w-full rounded-md border bg-transparent px-3 py-2 text-base shadow-xs transition-[color,box-shadow] outline-none focus-visible:ring-[3px] disabled:cursor-not-allowed disabled:opacity-50 md:text-sm";
        return MergeCss(baseClasses, Class ?? "");
    }

    private async Task OnChange(ChangeEventArgs args)
    {
        var newValue = args.Value?.ToString();
        Value = newValue;
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(newValue);
        }
    }
}

