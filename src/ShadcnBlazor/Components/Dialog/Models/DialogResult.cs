namespace ShadcnBlazor.Components.Dialog.Models;

/// <summary>
/// Result returned when a dialog is closed.
/// </summary>
public class DialogResult
{
    /// <summary>
    /// Data returned from the dialog (when not cancelled).
    /// </summary>
    public object? Data { get; set; }

    /// <summary>
    /// Whether the dialog was cancelled (e.g. by clicking outside or pressing Escape).
    /// </summary>
    public bool Cancelled { get; set; }

    /// <summary>
    /// Creates a successful result with the specified data.
    /// </summary>
    /// <param name="data">The data to return.</param>
    /// <returns>A dialog result indicating success.</returns>
    public static DialogResult Ok<T>(T data) => new()
    {
        Data = data,
        Cancelled = false
    };

    /// <summary>
    /// Creates a cancelled result.
    /// </summary>
    /// <returns>A dialog result indicating cancellation.</returns>
    public static DialogResult Cancel() => new()
    {
        Cancelled = true
    };
}
