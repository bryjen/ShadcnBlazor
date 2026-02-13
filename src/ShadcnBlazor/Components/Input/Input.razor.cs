using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Shared;
using ShadcnBlazor.Shared.Attributes;
using ShadcnBlazor.Shared.Enums;

namespace ShadcnBlazor.Components.Input;

[ComponentMetadata(Name = nameof(Input), Description = "Single-line text input with variant styling.", Dependencies = [])]
public partial class Input : ShadcnComponentBase
{
    [Parameter]
    public string? Type { get; set; } = "text";

    [Parameter]
    public string? Value { get; set; }

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    [Parameter]
    public Size Size { get; set; } = Size.Md;

    [Parameter]
    public bool Disabled { get; set; }

    [Parameter]
    public string? Placeholder { get; set; }

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
            Size.Xs => "h-7 px-2 py-1 text-xs",
            Size.Sm => "h-8 px-2.75 py-1.25 text-sm",
            Size.Md => "h-9 px-3 py-1.5 text-base md:text-sm",
            Size.Lg => "h-10 px-3.5 py-2 text-base",
            _ => "h-9 px-3 py-1.5 text-base md:text-sm",
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
