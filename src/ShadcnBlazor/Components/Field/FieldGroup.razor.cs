using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Components.Field;

/// <summary>
/// Container for stacking multiple fields.
/// </summary>
public partial class FieldGroup : ShadcnComponentBase
{
    /// <summary>The content of the field group.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string GetClass()
    {
        return MergeCss(
            "group/field-group @container/field-group flex w-full flex-col gap-7 " +
            "data-[slot=checkbox-group]:gap-3 [&>[data-slot=field-group]]:gap-4",
            Class);
    }
}
