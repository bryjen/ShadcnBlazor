using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Components.Input.InputGroup;

/// <summary>
/// Button for use within an input group.
/// </summary>
public partial class InputGroupButton : ShadcnComponentBase
{
    /// <summary>
    /// The content of the button.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Content)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The size variant of the button.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Appearance)]
    public InputGroupButtonSize ButtonSize { get; set; } = InputGroupButtonSize.Xs;

    /// <summary>
    /// Callback invoked when the button is clicked.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    private string GetClass()
    {
        var sizeClasses = ButtonSize switch
        {
            InputGroupButtonSize.Xs => "h-6 gap-1 rounded-[calc(var(--radius)-3px)] px-1.5 shadow-none text-sm",
            InputGroupButtonSize.Sm => "shadow-none text-sm",
            InputGroupButtonSize.IconXs => "size-6 rounded-[calc(var(--radius)-3px)] p-0 shadow-none",
            InputGroupButtonSize.IconSm => "size-8 p-0 shadow-none",
            _ => "h-6 gap-1 rounded-[calc(var(--radius)-3px)] px-1.5 shadow-none text-sm",
        };

        return MergeCss(sizeClasses, Class ?? string.Empty);
    }
}
