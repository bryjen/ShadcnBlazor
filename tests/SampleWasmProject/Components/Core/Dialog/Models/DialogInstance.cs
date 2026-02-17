namespace SampleWasmProject.Components.Core.Dialog.Models;

/// <summary>
/// Model representing a single dialog instance.
/// </summary>
public sealed class DialogInstance
{
    /// <summary>
    /// Unique identifier for this dialog.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Dialog title.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Type of the component to render as dialog content.
    /// </summary>
    public required Type ComponentType { get; init; }

    /// <summary>
    /// Parameters to pass to the dialog content component.
    /// </summary>
    public DialogParameters Parameters { get; init; } = new();

    /// <summary>
    /// Dialog options.
    /// </summary>
    public DialogOptions Options { get; init; } = new();

    /// <summary>
    /// Task completion source for the dialog result.
    /// </summary>
    public required TaskCompletionSource<DialogResult> TaskCompletionSource { get; init; }

    /// <summary>
    /// Reference handle for the dialog.
    /// </summary>
    public required DialogReference Reference { get; init; }
}
