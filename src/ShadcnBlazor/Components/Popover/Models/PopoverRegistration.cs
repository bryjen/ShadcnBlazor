using Microsoft.AspNetCore.Components;

namespace ShadcnBlazor.Components.Popover.Models;

internal sealed class PopoverRegistration
{
    public required string PopoverId { get; init; }
    public required string AnchorId { get; init; }
    public RenderFragment? Content { get; init; }
    public bool Render { get; init; }
    public bool Open { get; init; }
    public PopoverPlacement AnchorOrigin { get; init; }
    public PopoverPlacement TransformOrigin { get; init; }
    public PopoverWidthMode WidthMode { get; init; }
    public bool ClampList { get; init; }
    public string? PopoverClass { get; init; }
    public Dictionary<string, object>? PopoverAttributes { get; init; }
    /// <summary>
    /// Gap in pixels between the popover and its anchor. Applied in the direction away from the anchor.
    /// </summary>
    public int Offset { get; init; }

    /// <summary>
    /// A secondary offset along the alignment axis, independent of sideOffset.
    /// </summary>
    public int AlignOffset { get; init; }

    /// <summary>
    /// Hide the popover when the anchor element is scrolled completely out of the viewport.
    /// </summary>
    public bool HideWhenDetached { get; init; }
}
