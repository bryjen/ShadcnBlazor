using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ShadcnBlazor.Shared;
using ShadcnBlazor.Shared.Attributes;
using ShadcnBlazor.Shared.Enums;

namespace ShadcnBlazor.Components.Button;

[ComponentMetadata(Name = nameof(Button), Description = "Clickable button with variants (default, destructive, outline, secondary, ghost, link) and sizes.", Dependencies = [])]
public partial class Button : ShadcnComponentBase
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public Variant Variant { get; set; } = Variant.Default;

    [Parameter]
    public Size Size { get; set; } = Size.Md;

    [Parameter]
    public bool Disabled { get; set; }

    [Parameter]
    public ButtonType Type { get; set; } = ButtonType.Button;

    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    private string GetClass()
    {
        return ButtonStyles.Build(base.MergeCss, Variant, Size, Class);
    }
}