namespace ShadcnBlazor.Components.Sheet.Models;

/// <summary>
/// Result returned when a sheet is closed.
/// </summary>
public sealed class SheetResult
{
    /// <summary>
    /// Whether the sheet was confirmed (e.g. user selected an item).
    /// </summary>
    public bool Confirmed { get; init; }

    /// <summary>
    /// Optional data payload.
    /// </summary>
    public object? Data { get; init; }

    /// <summary>
    /// Creates a confirmed result with optional data.
    /// </summary>
    public static SheetResult Ok(object? data = null) => new() { Confirmed = true, Data = data };

    /// <summary>
    /// Creates a cancelled result.
    /// </summary>
    public static SheetResult Cancel() => new() { Confirmed = false };
}
