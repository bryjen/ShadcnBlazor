using Microsoft.AspNetCore.Components;
using SampleWasmProject.Components.Core.Shared;

namespace SampleWasmProject.Components.Core.Textarea;

/// <summary>
/// Multi-line text input with optional header and footer slots.
/// </summary>
public partial class ComposableTextArea : ShadcnComponentBase
{
    /// <summary>
    /// The current value of the text area.
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Callback invoked when the value changes.
    /// </summary>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// Callback invoked when the input change event fires.
    /// </summary>
    [Parameter]
    public EventCallback<ChangeEventArgs> OnChange { get; set; }

    /// <summary>
    /// The number of visible text rows.
    /// </summary>
    [Parameter]
    public int Rows { get; set; } = 8;

    /// <summary>
    /// Placeholder text when the value is empty.
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Whether the text area is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// CSS classes for the header container.
    /// </summary>
    [Parameter]
    public string HeaderContainerClass { get; set; } = string.Empty;

    /// <summary>
    /// CSS classes for the footer container.
    /// </summary>
    [Parameter]
    public string FooterContainerClass { get; set; } = string.Empty;

    /// <summary>
    /// Optional header content above the text area.
    /// </summary>
    [Parameter]
    public RenderFragment? Header { get; set; }

    /// <summary>
    /// Optional footer content below the text area.
    /// </summary>
    [Parameter]
    public RenderFragment? Footer { get; set; }

    private string GetContainerClass()
    {
        var baseClasses = "bg-input/30 border-input w-full min-w-0 rounded-xl border shadow-xs transition-all duration-200 " +
                          "focus-within:border-ring focus-within:ring-ring/50 focus-within:ring-[3px] " +
                          "aria-invalid:ring-destructive/40 aria-invalid:border-destructive";

        var stateClasses = Disabled
            ? "pointer-events-none cursor-not-allowed opacity-50"
            : string.Empty;

        return MergeCss(baseClasses, stateClasses, Class);
    }

    private string GetTextAreaClass()
    {
        var baseClasses = "w-full min-h-16 resize-none bg-transparent px-3 py-3 text-base md:text-sm outline-none border-0 " +
                          "placeholder:text-muted-foreground selection:bg-primary selection:text-primary-foreground";

        return MergeCss(baseClasses);
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
