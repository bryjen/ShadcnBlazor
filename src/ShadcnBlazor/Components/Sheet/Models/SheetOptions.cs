namespace ShadcnBlazor.Components.Sheet.Models;

/// <summary>Placement of the sheet.</summary>
public enum SheetSide
{
    /// <summary>Slide in from the left.</summary>
    Left,
    /// <summary>Slide in from the right.</summary>
    Right,
    /// <summary>Slide in from the top.</summary>
    Top,
    /// <summary>Slide in from the bottom.</summary>
    Bottom
}

/// <summary>Options that configure sheet behavior and appearance.</summary>
public sealed class SheetOptions
{
    /// <summary>Side of the viewport the sheet enters from.</summary>
    public SheetSide Side { get; set; } = SheetSide.Right;
    /// <summary>Optional size token or CSS value.</summary>
    public string? Size { get; set; }
}

