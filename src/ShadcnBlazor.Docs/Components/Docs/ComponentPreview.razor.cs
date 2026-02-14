using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Shared;

namespace ShadcnBlazor.Docs.Components.Docs;

public partial class ComponentPreview : ShadcnComponentBase
{
    [Parameter] 
    public RenderFragment? ChildContent { get; set; }
    [Parameter] 
    public string Code { get; set; } = "";
    [Parameter] 
    public string Language { get; set; } = "razor";
}