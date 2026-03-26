using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Components.Field;

/// <summary>
/// Title-style label text inside a field.
/// </summary>
public partial class FieldTitle : ShadcnComponentBase
{
    /// <summary>The title content.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string GetClass()
    {
        return MergeCss(
            "flex w-fit items-center gap-2 text-sm leading-snug font-medium group-data-[disabled=true]/field:opacity-50",
            Class);
    }
}
