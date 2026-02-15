using ShadcnBlazor.Components.Dialog.Models;

namespace ShadcnBlazor.Components.Dialog;

/// <summary>
/// Concrete implementation of <see cref="IDialogInstance"/> cascaded to dialog content.
/// </summary>
internal sealed class DialogInstanceCascade : IDialogInstance
{
    private readonly Action<DialogResult> _close;

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogInstanceCascade"/> class.
    /// </summary>
    /// <param name="close">Action to invoke when the dialog is closed.</param>
    public DialogInstanceCascade(Action<DialogResult> close)
    {
        _close = close;
    }

    /// <inheritdoc />
    public void Close(DialogResult result)
    {
        _close(result);
    }

    /// <inheritdoc />
    public void Close()
    {
        _close(DialogResult.Ok(true));
    }

    /// <inheritdoc />
    public void Cancel()
    {
        _close(DialogResult.Cancel());
    }
}
