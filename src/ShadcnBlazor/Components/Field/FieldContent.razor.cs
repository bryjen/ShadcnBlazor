using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Components.Field;

/// <summary>
/// Content container inside a field.
/// </summary>
public partial class FieldContent : ShadcnComponentBase
{
    /// <summary>The content of the field.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string GetClass()
    {
        return MergeCss("group/field-content flex flex-1 flex-col gap-1.5 leading-snug", Class);
    }
}
