namespace ShadcnBlazor.Components.Dialog;

/// <summary>
/// Reference to a shown dialog, used to await its result and close it programmatically.
/// </summary>
public interface IDialogReference
{
    /// <summary>
    /// Unique identifier of the dialog.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Task that completes when the dialog is closed.
    /// </summary>
    Task<Models.DialogResult> Result { get; }

    /// <summary>
    /// Closes the dialog with the specified result.
    /// </summary>
    /// <param name="result">The result to return.</param>
    void Close(Models.DialogResult result);
}
