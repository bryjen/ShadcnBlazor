using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Shared;
using ShadcnBlazor.Shared.Attributes;
using ShadcnBlazor.Shared.Enums;
using TailwindMerge;
#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.

namespace ShadcnBlazor.Components.Card;

[ComponentMetadata(Name = nameof(Card), Description = "Container for content with header, body, and footer sections.", Dependencies = [])]
public partial class Card : ShadcnComponentBase
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public CardVariant Variant { get; set; } = CardVariant.Default;

    private string GetClass()
    {
        var baseClasses = "text-card-foreground flex flex-col gap-6 rounded-xl border border-border p-4 shadow-sm transition-shadow duration-200 hover:shadow-md";
        var variantClass = Variant switch
        {
            CardVariant.Default => "bg-card",
            CardVariant.Outline => "bg-transparent",
        };

        return MergeCss(baseClasses, variantClass);
    }
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

