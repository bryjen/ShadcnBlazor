using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Shared;
using ShadcnBlazor.Shared.Attributes;

namespace ShadcnBlazor.Components.ComposableTextArea;

[ComponentMetadata(Name = nameof(ComposableTextArea), Description = "Multi-line text input with optional header and footer slots.", Dependencies = [])]
public partial class ComposableTextArea : ShadcnComponentBase
{
    [Parameter]
    public string? Value { get; set; }

    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    [Parameter]
    public EventCallback<ChangeEventArgs> OnChange { get; set; }

    [Parameter]
    public int Rows { get; set; } = 8;

    [Parameter]
    public string? Placeholder { get; set; }

    [Parameter]
    public bool Disabled { get; set; }

    [Parameter]
    public string HeaderContainerClass { get; set; } = string.Empty;
    
    [Parameter]
    public string FooterContainerClass { get; set; } = string.Empty;

    [Parameter]
    public RenderFragment? Header { get; set; }

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