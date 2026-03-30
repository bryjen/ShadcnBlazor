using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Components.Field;

/// <summary>
/// Fieldset wrapper for grouping related fields.
/// </summary>
public partial class FieldSet : ShadcnComponentBase
{
    /// <summary>The content of the fieldset.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string GetClass()
    {
        return MergeCss(
            "flex flex-col gap-6 " +
            "has-[>[data-slot=checkbox-group]]:gap-3 has-[>[data-slot=radio-group]]:gap-3",
            Class);
    }
}
