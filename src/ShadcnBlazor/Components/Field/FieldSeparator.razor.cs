using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Components.Field;

/// <summary>
/// Separator with optional inline content between field groups.
/// </summary>
public partial class FieldSeparator : ShadcnComponentBase
{
    /// <summary>The optional content rendered over the separator line.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string GetClass()
    {
        return MergeCss(
            "relative -my-2 h-5 text-sm group-data-[variant=outline]/field-group:-mb-2",
            Class);
    }
}
