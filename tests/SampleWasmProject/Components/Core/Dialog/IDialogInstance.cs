using SampleWasmProject.Components.Core.Dialog.Models;

namespace SampleWasmProject.Components.Core.Dialog;

/// <summary>
/// Interface cascaded to dialog content components, allowing them to close the dialog.
/// </summary>
public interface IDialogInstance
{
    /// <summary>
    /// Closes the dialog with the specified result.
    /// </summary>
    /// <param name="result">The result to return.</param>
    void Close(DialogResult result);

    /// <summary>
    /// Closes the dialog with a successful result (Ok with true).
    /// </summary>
    void Close();

    /// <summary>
    /// Cancels the dialog.
    /// </summary>
    void Cancel();
}
