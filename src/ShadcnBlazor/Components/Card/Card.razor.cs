using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Shared.Attributes;

namespace ShadcnBlazor.Components.Card;

[ComponentMetadata(Name = nameof(Card), Description = "", Dependencies = [])]
public partial class Card
{
    [Parameter] 
    public RenderFragment? ChildContent { get; set; }
    
    [Parameter] 
    public string? Class { get; set; }
    
    [Parameter(CaptureUnmatchedValues = true)] 
    public Dictionary<string, object>? AdditionalAttributes { get; set; }
}

