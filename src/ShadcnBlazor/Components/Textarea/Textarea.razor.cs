using Microsoft.AspNetCore.Components;
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
    public string? Value { get; set; }

    /// <summary>
    /// Callback invoked when the value changes.
    /// </summary>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// The number of visible text rows.
    /// </summary>
    [Parameter]
    public int Rows { get; set; } = 4;

    /// <summary>
    /// Placeholder text when the value is empty.
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Whether the textarea is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

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

