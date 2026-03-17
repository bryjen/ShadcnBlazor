namespace ShadcnBlazor.Components.Sheet.Models;

public enum SheetSide
{
    Left,
    Right,
    Top,
    Bottom
}

public sealed class SheetOptions
{
    public SheetSide Side { get; set; } = SheetSide.Right;
    public string? Size { get; set; }
}

