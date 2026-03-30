using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Components.Field;

/// <summary>
/// Helper text for a field.
/// </summary>
public partial class FieldDescription : ShadcnComponentBase
{
    /// <summary>The description content.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string GetClass()
    {
        return MergeCss(
            "text-sm leading-normal font-normal text-muted-foreground group-has-[[data-orientation=horizontal]]/field:text-balance " +
            "last:mt-0 nth-last-2:-mt-1 [[data-variant=legend]+&]:-mt-1.5 " +
            "[&>a]:underline [&>a]:underline-offset-4 [&>a:hover]:text-primary",
            Class);
    }
}
