using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Components.Input.InputGroup;

/// <summary>
/// Container for grouping Input with prefix/suffix addons, icons, and buttons.
/// </summary>
public partial class InputGroup : ShadcnComponentBase
{
    /// <summary>
    /// The content of the input group.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Content)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Whether the input group is disabled.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public bool Disabled { get; set; }

    private string GetClass()
    {
        var baseClasses = "group/input-group relative flex h-9 w-full min-w-0 items-center rounded-md border border-input " +
                          "shadow-xs transition-[color,box-shadow] outline-none dark:bg-input/30 " +
                          "has-[>[data-align=inline-start]]:[&>input]:pl-1.5 " +
                          "has-[>[data-align=inline-end]]:[&>input]:pr-1.5 " +
                          "has-[>[data-align=block-start]]:h-auto has-[>[data-align=block-start]]:flex-col has-[>[data-align=block-start]]:[&>input]:pb-3 " +
                          "has-[>[data-align=block-end]]:h-auto has-[>[data-align=block-end]]:flex-col has-[>[data-align=block-end]]:[&>input]:pt-3 " +
                          "has-[[data-slot=input-group-control]:focus-visible]:border-ring " +
                          "has-[[data-slot=input-group-control]:focus-visible]:ring-[3px] " +
                          "has-[[data-slot=input-group-control]:focus-visible]:ring-ring/50 " +
                          "has-[[data-slot][aria-invalid=true]]:border-destructive " +
                          "has-[[data-slot][aria-invalid=true]]:ring-destructive/20 " +
                          "dark:has-[[data-slot][aria-invalid=true]]:ring-destructive/40 " +
                          "has-[>textarea]:h-auto";

        var disabledClasses = Disabled ? "data-[disabled=true]:opacity-50 data-[disabled=true]:bg-input/50" : "";

        return MergeCss(baseClasses, disabledClasses, Class ?? string.Empty);
    }
}
