namespace ShadcnBlazor.Components.Sheet.Models;

/// <summary>
/// Model representing a single sheet instance.
/// </summary>
public sealed class SheetInstance
{
    /// <summary>
    /// Unique identifier for this sheet.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Type of the component to render as sheet content.
    /// </summary>
    public required Type ComponentType { get; init; }

    /// <summary>
    /// Parameters to pass to the sheet content component.
    /// </summary>
    public SheetParameters Parameters { get; init; } = new();

    /// <summary>
    /// Task completion source for the sheet result.
    /// </summary>
    public required TaskCompletionSource<SheetResult> TaskCompletionSource { get; init; }

    /// <summary>
    /// Reference handle for the sheet.
    /// </summary>
    public required SheetReference Reference { get; init; }

    /// <summary>
    /// Close handler registered by the container when it mounts.
    /// </summary>
    internal Func<SheetResult, Task>? CloseHandler { get; set; }
}
