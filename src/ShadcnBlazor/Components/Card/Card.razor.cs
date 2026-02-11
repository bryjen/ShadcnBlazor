using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Shared;
using ShadcnBlazor.Shared.Attributes;
using ShadcnBlazor.Shared.Enums;
#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.

namespace ShadcnBlazor.Components.Card;

[ComponentMetadata(Name = nameof(Card), Description = "", Dependencies = [])]
public partial class Card
{
    [Parameter] 
    public RenderFragment? ChildContent { get; set; }

    [Parameter] 
    public string Class { get; set; } = string.Empty;
    
    [Parameter]
    public CardVariant Variant { get; set; } = CardVariant.Default;
    
    [Parameter(CaptureUnmatchedValues = true)] 
    public Dictionary<string, object>? AdditionalAttributes { get; set; }
    
    private string GetClass()
    {
        var baseClasses = "text-card-foreground flex flex-col gap-6 rounded-xl border border-border p-4 shadow-sm transition-shadow duration-200 hover:shadow-md";
        var variantClass = Variant switch
        {
            CardVariant.Default => "bg-card",
            CardVariant.Outline => "bg-transparent",
        };
        
        return ClassBuilder.Merge(baseClasses, variantClass, Class);
    }
}

public enum CardVariant  
{
    Default,
    Outline,
}

