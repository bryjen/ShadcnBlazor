using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Shared.Attributes;
using ShadcnBlazor.Shared.Enums;

#pragma warning disable CS8524

namespace ShadcnBlazor.Components.Radio;

[ComponentMetadata(Name = nameof(Radio), Description = "Radio and RadioCard options for single selection within a RadioGroup.", Dependencies = [])]
public partial class Radio : RadioSelectableComponentBase
{
    [Parameter]
    public RenderFragment? LabelContent { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string ButtonClass { get; set; } = string.Empty;

    private string GetContainerClass()
    {
        return MergeCss("flex items-start gap-2 cursor-pointer", Class);
    }

    private string GetClass()
    {
        var baseClasses = "peer border-input bg-input/30 data-[state=checked]:bg-primary data-[state=checked]:text-primary-foreground data-[state=checked]:border-primary focus-visible:border-ring focus-visible:ring-ring/50 aria-invalid:ring-destructive/40 aria-invalid:border-destructive shrink-0 rounded-full border shadow-xs transition-all duration-200 outline-none focus-visible:ring-[3px] disabled:cursor-not-allowed disabled:opacity-50";
        var sizeClasses = EffectiveSize switch
        {
            Size.Xs => "size-2",
            Size.Sm => "size-3",
            Size.Md => "size-4",
            Size.Lg => "size-5",
        };

        return MergeCss(baseClasses, sizeClasses, ButtonClass);
    }

    private string GetIndicatorClass()
    {
        if (IsChecked)
        {
            return "grid place-content-center size-full text-current animate-in zoom-in-50 fade-in-0 duration-200";
        }

        return "grid place-content-center size-full text-current animate-out zoom-out-50 fade-out-0 duration-150 pointer-events-none opacity-0";
    }

    private string GetDotClass()
    {
        var sizeClasses = EffectiveSize switch
        {
            Size.Xs => "size-0.5",
            Size.Sm => "size-1",
            Size.Md => "size-1.5",
            Size.Lg => "size-2",
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




