namespace ShadcnBlazor.Components.Popover.Models;

/// <summary>
/// Configuration options for the Popover JavaScript interop.
/// </summary>
public class PopoverOptions
{
    /// <summary>
    /// CSS class applied to the popover container element.
    /// </summary>
    public string ContainerClass { get; set; } = "popover-provider";

    /// <summary>
    /// Margin used when flipping the popover to stay in viewport.
    /// </summary>
    public int FlipMargin { get; set; } = 0;

    /// <summary>
    /// Padding from viewport edges when positioning.
    /// </summary>
    public int OverflowPadding { get; set; } = 10;

    /// <summary>
    /// Base z-index for popover elements.
    /// </summary>
    public int BaseZIndex { get; set; } = 1200;
}
