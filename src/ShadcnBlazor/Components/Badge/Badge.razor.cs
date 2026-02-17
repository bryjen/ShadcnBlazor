using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;
using ShadcnBlazor.Components.Shared.Models.Enums;

#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.

namespace ShadcnBlazor.Components.Badge;

/// <summary>
/// A small label or count indicator for highlighting status or metadata.
/// </summary>
public partial class Badge : ShadcnComponentBase
{
    /// <summary>The content of the badge.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>The visual style variant of the badge.</summary>
    [Parameter]
    public Variant Variant { get; set; } = Variant.Default;

    /// <summary>The size of the badge.</summary>
    [Parameter]
    public Size Size { get; set; } = Size.Md;

    private string GetClass()
    {
        var baseClasses = "inline-flex items-center justify-center rounded-full border w-fit whitespace-nowrap shrink-0 gap-1 [&>svg]:pointer-events-none focus-visible:border-ring focus-visible:ring-ring/50 focus-visible:ring-[3px] aria-invalid:ring-destructive/40 aria-invalid:border-destructive transition-all duration-200 overflow-hidden";

        var sizeClasses = Size switch
        {
            Size.Sm => "px-2 py-0.5 text-[0.6rem] [&>svg]:size-2",
            Size.Md => "px-2.5 py-0.5 text-xs [&>svg]:size-2.5",
            Size.Lg => "px-2.5 py-1 text-sm [&>svg]:size-3",
        };

        var variantClasses = Variant switch
        {
            Variant.Default => "border-transparent bg-primary text-primary-foreground [a&]:hover:bg-primary/90",
            Variant.Secondary => "border-transparent bg-secondary text-secondary-foreground [a&]:hover:bg-secondary/90",
            Variant.Destructive => "border-transparent bg-destructive/60 text-white [a&]:hover:bg-destructive/90 focus-visible:ring-destructive/40",
            Variant.Outline => "border-border text-foreground [a&]:hover:bg-accent [a&]:hover:text-accent-foreground",
            Variant.Ghost => "border-transparent [a&]:hover:bg-accent [a&]:hover:text-accent-foreground",
            Variant.Link => "border-transparent text-primary underline-offset-4 [a&]:hover:underline",
        };

        return MergeCss(baseClasses, sizeClasses, variantClasses, Class ?? "");
    }
}

