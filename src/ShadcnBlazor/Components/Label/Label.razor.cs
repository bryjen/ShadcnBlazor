using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Components.Label;

/// <summary>
/// A styled label element for form controls.
/// </summary>
public partial class Label : ShadcnComponentBase
{
    /// <summary>
    /// The content rendered inside the label.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The id of the form control this label is associated with.
    /// </summary>
    [Parameter]
    public string? For { get; set; }

    private string GetClass()
    {
        return MergeCss(
            "flex items-center gap-2 text-sm leading-none font-medium select-none " +
            "group-data-[disabled=true]:pointer-events-none group-data-[disabled=true]:opacity-50 " +
            "peer-disabled:cursor-not-allowed peer-disabled:opacity-50",
            Class);
    }
}
