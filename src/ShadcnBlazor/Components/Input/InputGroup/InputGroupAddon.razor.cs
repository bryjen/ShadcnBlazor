using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Components.Input.InputGroup;

/// <summary>
/// Addon container for input group (prefix or suffix content).
/// </summary>
public partial class InputGroupAddon : ShadcnComponentBase
{
    /// <summary>
    /// The content of the addon.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Content)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The alignment position of the addon.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Appearance)]
    public InputGroupAddonAlign Align { get; set; } = InputGroupAddonAlign.InlineStart;

    private string GetAlignValue() => Align switch
    {
        InputGroupAddonAlign.InlineStart => "inline-start",
        InputGroupAddonAlign.InlineEnd => "inline-end",
        InputGroupAddonAlign.BlockStart => "block-start",
        InputGroupAddonAlign.BlockEnd => "block-end",
        _ => "inline-start",
    };

    private string GetClass()
    {
        var baseClasses = "flex h-auto cursor-text items-center justify-center gap-2 py-1.5 text-sm font-medium text-muted-foreground " +
                          "select-none group-data-[disabled=true]/input-group:opacity-50 " +
                          "[&>svg:not([class*='size-'])]:size-4";

        var alignmentClasses = Align switch
        {
            InputGroupAddonAlign.InlineStart => "order-first pl-2",
            InputGroupAddonAlign.InlineEnd => "order-last pr-2",
            InputGroupAddonAlign.BlockStart => "order-first w-full justify-start px-2.5 pt-2",
            InputGroupAddonAlign.BlockEnd => "order-last w-full justify-start px-2.5 pb-2",
            _ => "order-first pl-2",
        };

        return MergeCss(baseClasses, alignmentClasses, Class ?? string.Empty);
    }
}
