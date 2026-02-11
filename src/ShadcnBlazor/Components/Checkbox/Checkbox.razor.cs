using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Shared;
using ShadcnBlazor.Shared.Attributes;
using ShadcnBlazor.Shared.Enums;
using TailwindMerge;

#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.

namespace ShadcnBlazor.Components.Checkbox;

[ComponentMetadata(Name = nameof(Checkbox), Description = "", Dependencies = [])]
public partial class Checkbox : ShadcnComponentBase
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public bool Checked { get; set; }

    [Parameter]
    public EventCallback<bool> CheckedChanged { get; set; }

    [Parameter]
    public Size Size { get; set; } = Size.Md;

    [Parameter]
    public bool Disabled { get; set; }
    
    private string GetClass()
    {
        var baseClasses = "peer border-input bg-input/30 data-[state=checked]:bg-primary data-[state=checked]:text-primary-foreground data-[state=checked]:border-primary focus-visible:border-ring focus-visible:ring-ring/50 aria-invalid:ring-destructive/40 aria-invalid:border-destructive shrink-0 rounded-[4px] border shadow-xs transition-all duration-200 outline-none focus-visible:ring-[3px] disabled:cursor-not-allowed disabled:opacity-50";
        var sizeClasses = Size switch
        {
            Size.Xs => "size-2",
            Size.Sm => "size-3",
            Size.Md => "size-4",
            Size.Lg => "size-5",
        };

        return MergeCss(baseClasses, sizeClasses, Class ?? "");
    }

    private string GetIndicatorClass()
    {
        if (Checked)
        {
            return "grid place-content-center text-current animate-in zoom-in-50 fade-in-0 duration-200";
        }
        else
        {
            return "grid place-content-center text-current animate-out zoom-out-50 fade-out-0 duration-150 pointer-events-none opacity-0";
        }
    }

    private string GetAriaChecked()
    {
        return Checked ? "true" : "false";
    }

    private async Task HandleClick()
    {
        if (!Disabled)
        {
            Checked = !Checked;
            await CheckedChanged.InvokeAsync(Checked);
        }
    }
}

