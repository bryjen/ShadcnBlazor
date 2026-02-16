using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Docs.Components.Docs.CodeBlock;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Docs.Components.Docs;

public partial class ComponentPreview : ShadcnComponentBase
{
    [Parameter] 
    public RenderFragment? ChildContent { get; set; }
    
    [Parameter] 
    public IReadOnlyList<CodeFile>? CodeFiles { get; set; }
}