using Microsoft.AspNetCore.Components;
using SampleWasmProject.Components.Core.Shared;
using TailwindMerge;
#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.

namespace SampleWasmProject.Components.Core.Alert;

public partial class Alert : ShadcnComponentBase
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public AlertVariant Variant { get; set; } = AlertVariant.Default;

    private string GetClass()
    {
        var baseClasses = "relative w-full rounded-lg border px-4 py-3 text-sm grid has-[>svg]:grid-cols-[auto_1fr] has-[>svg]:gap-x-3 gap-y-1 items-start [&>svg]:size-4 [&>svg]:shrink-0 [&>svg]:translate-y-0.5 animate-in fade-in-0 slide-in-from-top-1 duration-200";

        var variantClasses = Variant switch
        {
            AlertVariant.Default => "bg-card text-card-foreground border-border [&>svg]:text-card-foreground",
            AlertVariant.Destructive => "bg-destructive/20 text-card-foreground border-destructive [&>svg]:text-destructive *:data-[slot=alert-description]:text-destructive/90",
        };

        return MergeCss(baseClasses, variantClasses, Class);
    }
}

/// <summary>
/// Visual style variants for the Alert component.
/// </summary>
public enum AlertVariant
{
    /// <summary>Default informational styling.</summary>
    Default,

    /// <summary>Destructive/error styling for alerts or warnings.</summary>
    Destructive,
}

