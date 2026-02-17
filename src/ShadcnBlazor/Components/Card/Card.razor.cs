using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;

#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.

namespace ShadcnBlazor.Components.Card;

/// <summary>
/// A container card with optional header and footer sections.
/// </summary>
public partial class Card : ShadcnComponentBase
{
    /// <summary>The main content of the card.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>Optional header content rendered above the main content.</summary>
    [Parameter]
    public RenderFragment? Header { get; set; }

    /// <summary>Optional footer content rendered below the main content.</summary>
    [Parameter]
    public RenderFragment? Footer { get; set; }

    /// <summary>The visual style variant of the card.</summary>
    [Parameter]
    public CardVariant Variant { get; set; } = CardVariant.Default;

    /// <summary>The size/spacing of the card sections.</summary>
    [Parameter]
    public CardSize Size { get; set; } = CardSize.Default;

    /// <summary>Additional CSS classes for the header section.</summary>
    [Parameter]
    public string HeaderClass { get; set; } = string.Empty;

    /// <summary>Additional CSS classes for the content section.</summary>
    [Parameter]
    public string ContentClass { get; set; } = string.Empty;

    /// <summary>Additional CSS classes for the footer section.</summary>
    [Parameter]
    public string FooterClass { get; set; } = string.Empty;

    private bool HasHeaderOrFooter => Header is not null || Footer is not null;

    private string GetContainerClass()
    {
        var variantClass = Variant switch
        {
            CardVariant.Default => "bg-card",
            CardVariant.Outline => "bg-transparent",
        };

        if (!HasHeaderOrFooter)
        {
            var gapClass = Size == CardSize.Sm ? "gap-4" : "gap-6";
            var paddingClass = Size == CardSize.Sm ? "p-3" : "p-4";
            return MergeCss($"text-card-foreground flex flex-col {gapClass} rounded-xl border border-border {paddingClass} shadow-sm transition-shadow duration-200 hover:shadow-md", variantClass);
        }

        return MergeCss("text-card-foreground flex flex-col rounded-xl border border-border overflow-hidden shadow-sm transition-shadow duration-200 hover:shadow-md", variantClass);
    }

    private string GetSectionPadding() => Size == CardSize.Sm ? "p-3" : "p-4";

    private string GetSectionGap() => Size == CardSize.Sm ? "gap-4" : "gap-6";

    private string GetHeaderClass() => MergeCssNoUserClass("flex flex-col space-y-1.5 border-b border-border", GetSectionPadding(), HeaderClass);

    private string GetContentClass() => MergeCssNoUserClass("flex flex-col flex-1", GetSectionGap(), GetSectionPadding(), ContentClass);

    private string GetFooterClass() => MergeCssNoUserClass("flex items-center border-t border-border bg-muted/50", GetSectionPadding(), FooterClass);
}

/// <summary>
/// Visual style variants for the Card component.
/// </summary>
public enum CardVariant
{
    /// <summary>Default card with background.</summary>
    Default,

    /// <summary>Outline style with transparent background.</summary>
    Outline,
}

/// <summary>
/// Size variants for the Card component.
/// </summary>
public enum CardSize
{
    /// <summary>Default spacing.</summary>
    Default,

    /// <summary>Smaller, more compact spacing.</summary>
    Sm,
}
