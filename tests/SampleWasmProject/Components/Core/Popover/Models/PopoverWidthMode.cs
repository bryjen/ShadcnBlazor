namespace SampleWasmProject.Components.Core.Popover.Models;

/// <summary>
/// How the popover width is determined relative to the anchor.
/// </summary>
public enum PopoverWidthMode
{
    /// <summary>No width constraint; use content width.</summary>
    None,

    /// <summary>Match anchor width.</summary>
    Relative,

    /// <summary>Adapt width based on content and viewport.</summary>
    Adaptive
}
