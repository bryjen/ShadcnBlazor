using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Components.Field;

/// <summary>
/// Styled field label using the Label component.
/// </summary>
public partial class FieldLabel : ShadcnComponentBase
{
    /// <summary>The label content.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>The id of the associated form control.</summary>
    [Parameter]
    public string? For { get; set; }

    [CascadingParameter]
    private Field? ParentField { get; set; }

    internal string? ResolvedFor => For ?? ParentField?.FieldId;

    private string GetClass()
    {
        return MergeCss(
            "group/field-label peer/field-label flex w-fit gap-2 leading-snug group-data-[disabled=true]/field:opacity-50 " +
            "has-[>[data-slot=field]]:w-full has-[>[data-slot=field]]:flex-col has-[>[data-slot=field]]:rounded-md " +
            "has-[>[data-slot=field]]:border [&>*]:data-[slot=field]:p-4 " +
            "has-data-[state=checked]:border-primary has-data-[state=checked]:bg-primary/5 dark:has-data-[state=checked]:bg-primary/10",
            Class);
    }
}
