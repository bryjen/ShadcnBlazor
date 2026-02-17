using Microsoft.AspNetCore.Components;
using SampleWasmProject.Components.Core.Shared.Models.Enums;

#pragma warning disable CS8524

namespace SampleWasmProject.Components.Core.Radio;

/// <summary>
/// Card-style radio option for single selection within a RadioGroup.
/// </summary>
public partial class RadioCard : RadioSelectableComponentBase
{
    /// <summary>
    /// Content displayed inside the radio card.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string GetClass()
    {
        var baseClasses = "w-full border-input bg-background text-foreground hover:bg-accent/20 data-[state=checked]:border-primary data-[state=checked]:bg-primary/15 focus-visible:border-ring focus-visible:ring-ring/50 aria-invalid:ring-destructive/40 aria-invalid:border-destructive inline-flex items-start justify-between gap-4 rounded-xl border p-4 shadow-xs transition-all duration-200 outline-none focus-visible:ring-[3px] disabled:cursor-not-allowed disabled:opacity-50";
        return MergeCss(baseClasses, Class ?? string.Empty);
    }

    private string GetIndicatorShellClass()
    {
        var sizeClasses = EffectiveSize switch
        {
            Size.Sm => "size-4",
            Size.Md => "size-5",
            Size.Lg => "size-6",
        };

        var baseClasses = "border-input bg-input/20 shrink-0 rounded-full border inline-flex items-center justify-center";
        return MergeCss(baseClasses, sizeClasses);
    }

    private string GetIndicatorClass()
    {
        if (IsChecked)
        {
            return "grid place-content-center size-full text-primary animate-in zoom-in-50 fade-in-0 duration-200";
        }

        return "grid place-content-center size-full text-primary animate-out zoom-out-50 fade-out-0 duration-150 pointer-events-none opacity-0";
    }

    private string GetDotClass()
    {
        var sizeClasses = EffectiveSize switch
        {
            Size.Sm => "size-1.5",
            Size.Md => "size-2",
            Size.Lg => "size-2.5",
        };

        return MergeCss("rounded-full bg-current", sizeClasses);
    }

    private string GetAriaChecked()
    {
        return IsChecked ? "true" : "false";
    }

    private async Task HandleClick()
    {
        await SelectAsync();
    }
}
