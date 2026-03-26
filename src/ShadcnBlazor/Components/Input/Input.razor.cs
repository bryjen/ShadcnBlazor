using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using ShadcnBlazor.Components.Field;
using ShadcnBlazor.Components.Shared;
using ShadcnBlazor.Components.Shared.Models.Enums;

namespace ShadcnBlazor.Components.Input;

/// <summary>
/// Single-line text input with variant styling.
/// </summary>
public partial class Input : ShadcnComponentBase
{
    /// <summary>
    /// The HTML input type (e.g., text, password, email).
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public string? Type { get; set; } = "text";

    /// <summary>
    /// The current value of the input.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Data)]
    public string? Value { get; set; }

    /// <summary>
    /// Callback invoked when the value changes.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public EventCallback<string> ValueChanged { get; set; }

    /// <summary>
    /// The size of the input.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Appearance)]
    public Size Size { get; set; } = Size.Md;

    /// <summary>
    /// Whether the input is disabled.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public bool Disabled { get; set; }

    /// <summary>
    /// Placeholder text when the value is empty.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Common)]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Whether the input is required.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public bool Required { get; set; }

    /// <summary>
    /// Callback invoked when the input change event fires.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public EventCallback<ChangeEventArgs> OnChange { get; set; }

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

    private string GetClass()
    {
        var baseClasses = "w-full min-w-0 rounded-md border border-input bg-transparent px-3 py-1 text-base shadow-xs " +
                          "transition-[color,box-shadow] outline-none selection:bg-primary selection:text-primary-foreground " +
                          "file:inline-flex file:h-7 file:border-0 file:bg-transparent file:text-sm file:font-medium file:text-foreground " +
                          "placeholder:text-muted-foreground disabled:pointer-events-none disabled:cursor-not-allowed disabled:opacity-50 " +
                          "md:text-sm dark:bg-input/30 " +
                          "focus-visible:border-ring focus-visible:ring-[3px] focus-visible:ring-ring/50 " +
                          "aria-invalid:border-destructive aria-invalid:ring-destructive/20 dark:aria-invalid:ring-destructive/40";
        var sizeClasses = Size switch
        {
            Size.Sm => "h-8",
            Size.Md => "h-9",
            Size.Lg => "h-10",
            _ => "h-9",
        };

        return MergeCss(baseClasses, sizeClasses, Class ?? string.Empty);
    }

    private async Task HandleInput(ChangeEventArgs e)
    {
        Value = e.Value?.ToString();
        await ValueChanged.InvokeAsync(Value);
    }

    private async Task HandleChange(ChangeEventArgs e)
    {
        await OnChange.InvokeAsync(e);
    }
}
