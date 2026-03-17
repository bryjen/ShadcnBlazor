using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;
using ShadcnBlazor.Components.Shared.Models.Enums;
using TailwindMerge;
#pragma warning disable CS8524

namespace ShadcnBlazor.Components.Badge;

/// <summary>
/// Renders a badge as a link. Use for clickable badges that navigate.
/// </summary>
public partial class BadgeLink : ShadcnComponentBase
{
    /// <summary>The content of the badge link.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }
    /// <summary>The URL the badge links to.</summary>
    [Parameter] public string? Href { get; set; }
    /// <summary>The target attribute for the link (e.g. "_blank").</summary>
    [Parameter] public string? Target { get; set; }
    /// <summary>The visual style variant of the badge.</summary>
    [Parameter] public Variant Variant { get; set; } = Variant.Link;
    /// <summary>The size of the badge.</summary>
    [Parameter] public Size Size { get; set; } = Size.Md;

    private string GetClass()
    {
        var baseClasses = "inline-flex items-center justify-center rounded-full border w-fit whitespace-nowrap shrink-0 gap-1 [&>svg]:pointer-events-none no-underline transition-all duration-200 overflow-hidden";

        var sizeClasses = Size switch
        {
            Size.Sm => "px-2 py-0.5 text-[0.6rem] [&>svg]:size-2",
            Size.Md => "px-2.5 py-0.5 text-xs [&>svg]:size-2.5",
            Size.Lg => "px-2.5 py-1 text-sm [&>svg]:size-3",
        };

        var variantClasses = Variant switch
        {
            Variant.Default => "border-transparent bg-primary text-primary-foreground hover:bg-primary/90",
            Variant.Secondary => "border-transparent bg-secondary text-secondary-foreground hover:bg-secondary/90",
            Variant.Destructive => "border-transparent bg-destructive/60 text-white hover:bg-destructive/90",
            Variant.Outline => "border-border text-foreground hover:bg-accent hover:text-accent-foreground",
            Variant.Ghost => "border-transparent hover:bg-accent hover:text-accent-foreground",
            Variant.Link => "border-transparent text-primary underline-offset-4 hover:underline",
        };

        return MergeCss(baseClasses, sizeClasses, variantClasses, Class ?? "");
    }
}
