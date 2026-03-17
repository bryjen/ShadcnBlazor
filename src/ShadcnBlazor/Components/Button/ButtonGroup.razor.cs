using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;
using ShadcnBlazor.Components.Shared.Models.Enums;

namespace ShadcnBlazor.Components.Button;

/// <summary>
/// Groups multiple buttons together with shared styling and no gaps between them.
/// </summary>
public partial class ButtonGroup : ShadcnComponentBase
{
    /// <summary>The buttons to display in the group.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>The ARIA role for the group (default: "group").</summary>
    [Parameter]
    public string Role { get; set; } = "group";

    /// <summary>The visual style variant applied to the group and its buttons.</summary>
    [Parameter]
    public Variant Variant { get; set; } = Variant.Outline;

    private string GetClass()
    {
        var baseClasses = "inline-flex items-center overflow-hidden rounded-xl border shadow-xs " +
                          "[&>[data-slot=button]]:border-0 [&>[data-slot=button]]:shadow-none [&>[data-slot=button]]:rounded-none " +
                          "[&>[data-slot=button]]:focus-visible:relative [&>[data-slot=button]]:focus-visible:z-10 " +
                          "[&>[data-slot=button]]:transition-all [&>[data-slot=button]]:duration-200 [&>[data-slot=button]]:active:scale-95 " +
                          "[&>[data-slot=button]:not(:first-child)]:border-l " +
                          "[&>[data-slot=button]:first-child]:rounded-l-lg [&>[data-slot=button]:last-child]:rounded-r-lg";

        var variantClasses = Variant switch
        {
            Variant.Default => "border-primary bg-primary text-primary-foreground " +
                               "[&>[data-slot=button]]:bg-primary [&>[data-slot=button]]:text-primary-foreground [&>[data-slot=button]]:hover:bg-primary/90 " +
                               "[&>[data-slot=button]:not(:first-child)]:border-primary/35",
            Variant.Destructive => "border-destructive bg-destructive/80 text-white " +
                                   "[&>[data-slot=button]]:bg-destructive/80 [&>[data-slot=button]]:text-white [&>[data-slot=button]]:hover:bg-destructive/95 " +
                                   "[&>[data-slot=button]:not(:first-child)]:border-destructive/45",
            Variant.Outline => "border-input bg-background text-foreground " +
                               "[&>[data-slot=button]]:bg-background [&>[data-slot=button]]:text-foreground [&>[data-slot=button]]:hover:bg-accent [&>[data-slot=button]]:hover:text-accent-foreground " +
                               "[&>[data-slot=button]:not(:first-child)]:border-input",
            Variant.Secondary => "border-secondary bg-secondary text-secondary-foreground " +
                                 "[&>[data-slot=button]]:bg-secondary [&>[data-slot=button]]:text-secondary-foreground [&>[data-slot=button]]:hover:bg-secondary/80 " +
                                 "[&>[data-slot=button]:not(:first-child)]:border-secondary/45",
            Variant.Ghost => "border-input/70 bg-transparent text-foreground " +
                             "[&>[data-slot=button]]:bg-transparent [&>[data-slot=button]]:text-foreground [&>[data-slot=button]]:hover:bg-accent/40 " +
                             "[&>[data-slot=button]:not(:first-child)]:border-input/70",
            Variant.Link => "border-transparent bg-transparent text-primary " +
                            "[&>[data-slot=button]]:bg-transparent [&>[data-slot=button]]:text-primary [&>[data-slot=button]]:hover:underline " +
                            "[&>[data-slot=button]:not(:first-child)]:border-primary/20",
            _ => "border-input bg-background text-foreground [&>[data-slot=button]:not(:first-child)]:border-input"
        };

        return MergeCss(baseClasses, variantClasses, Class);
    }
}
