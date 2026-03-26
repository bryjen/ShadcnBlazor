using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Components.Input.InputGroup;

/// <summary>
/// Text content for an input group.
/// </summary>
public partial class InputGroupText : ShadcnComponentBase
{
    /// <summary>
    /// The content of the text element.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Content)]
    public RenderFragment? ChildContent { get; set; }

    private string GetClass()
    {
        var baseClasses = "flex items-center gap-2 text-sm text-muted-foreground " +
                          "[&_svg]:pointer-events-none [&_svg:not([class*='size-'])]:size-4";

        return MergeCss(baseClasses, Class ?? string.Empty);
    }
}
