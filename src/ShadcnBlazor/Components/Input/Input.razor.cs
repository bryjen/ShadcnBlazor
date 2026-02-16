using Microsoft.AspNetCore.Components;
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
    public string? Type { get; set; } = "text";

    /// <summary>
    /// The current value of the input.
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Callback invoked when the value changes.
    /// </summary>
    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    /// <summary>
    /// The size of the input.
    /// </summary>
    [Parameter]
    public Size Size { get; set; } = Size.Md;

    /// <summary>
    /// Whether the input is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Placeholder text when the value is empty.
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Callback invoked when the input change event fires.
    /// </summary>
    [Parameter]
    public EventCallback<ChangeEventArgs> OnChange { get; set; }

    private string GetClass()
    {
        var baseClasses = "placeholder:text-muted-foreground selection:bg-primary selection:text-primary-foreground " +
                          "bg-input/30 border-input w-full min-w-0 rounded-md border shadow-xs transition-all duration-200 " +
                          "outline-none disabled:pointer-events-none disabled:cursor-not-allowed disabled:opacity-50 " +
                          "focus-visible:border-ring focus-visible:ring-ring/50 focus-visible:ring-[3px] " +
                          "aria-invalid:ring-destructive/40 aria-invalid:border-destructive";
        var sizeClasses = Size switch
        {
            Size.Sm => "h-6 px-1.75 py-0.75 text-[0.6rem]",
            Size.Md => "h-7 px-2.5 py-1 text-sm",
            Size.Lg => "h-8 px-2.75 py-1.25 text-base md:text-sm",
            _ => "h-7 px-2.5 py-1 text-sm",
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
