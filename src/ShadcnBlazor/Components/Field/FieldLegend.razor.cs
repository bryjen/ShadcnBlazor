using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;
using ShadcnBlazor.Components.Shared.Models.Enums;

namespace ShadcnBlazor.Components.Field;

/// <summary>
/// Legend element for a fieldset.
/// </summary>
public partial class FieldLegend : ShadcnComponentBase
{
    /// <summary>The legend content.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>The legend style variant.</summary>
    [Parameter]
    public FieldLegendVariant Variant { get; set; } = FieldLegendVariant.Legend;

    private string GetClass()
    {
        return MergeCss(
            "mb-3 font-medium " +
            "data-[variant=legend]:text-base " +
            "data-[variant=label]:text-sm",
            Class);
    }
}
