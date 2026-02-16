using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SampleWasmProject.Components.Core.Shared;
using SampleWasmProject.Components.Core.Shared.Models.Enums;

namespace SampleWasmProject.Components.Core.Button;

public partial class Button : ShadcnComponentBase
{
    /// <summary>
    /// The content rendered inside the button.
    /// </summary>
    [Parameter]
    [Category("Content")]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The visual style variant of the button.
    /// </summary>
    [Parameter]
    [Category("Appearance")]
    public Variant Variant { get; set; } = Variant.Default;

    /// <summary>
    /// The size of the button.
    /// </summary>
    [Parameter]
    [Category("Appearance")]
    public Size Size { get; set; } = Size.Md;

    /// <summary>
    /// If true, the button is disabled and cannot be clicked.
    /// </summary>
    [Parameter]
    [Category("Behavior")]
    public bool Disabled { get; set; }

    /// <summary>
    /// The HTML button type (button, submit, or reset).
    /// </summary>
    [Parameter]
    [Category("Behavior")]
    public ButtonType Type { get; set; } = ButtonType.Button;

    /// <summary>
    /// Callback fired when the button is clicked.
    /// </summary>
    [Parameter]
    [Category("Behavior")]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    private string GetClass()
    {
        return ButtonStyles.Build(base.MergeCss, Variant, Size, Class);
    }
}