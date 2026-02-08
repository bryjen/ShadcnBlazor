using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ShadcnBlazor.ComponentDependencies;

namespace ShadcnBlazor.Components.Button;

[ComponentMetadata(Name = nameof(Button), Description = "", Dependencies = [])]
public partial class Button : ComponentBase
{
    [Parameter] 
    public RenderFragment? ChildContent { get; set; }

    [Parameter] 
    public string Variant { get; set; } = "default";

    [Parameter] 
    public string Size { get; set; } = "default";

    [Parameter] 
    public string? Class { get; set; }

    [Parameter] 
    public bool Disabled { get; set; }

    [Parameter] 
    public string Type { get; set; } = "button";

    [Parameter] 
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClass()
    {
        return ButtonStyles.Build(Variant, Size, Class);
    }
}