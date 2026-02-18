using ShadcnBlazor.Components.Dialog.Models;
using ShadcnBlazor.Components.Dialog.Services;

namespace ShadcnBlazor.Components.Dialog;

/// <summary>
/// Reference to a shown dialog.
/// </summary>
public sealed class DialogReference : IDialogReference
{
    private readonly DialogService _dialogService;

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public Task<DialogResult> Result =>
        _dialogService.DialogInstances
            .FirstOrDefault(x => x.Id == Id)
            ?.TaskCompletionSource.Task
        ?? Task.FromResult(DialogResult.Cancel());

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogReference"/> class.
    /// </summary>
    /// <param name="id">The dialog identifier.</param>
    /// <param name="dialogService">The dialog service.</param>
    public DialogReference(Guid id, DialogService dialogService)
    {
        Id = id;
        _dialogService = dialogService;
    }

    /// <inheritdoc />
    public void Close(DialogResult result)
    {
        _ = _dialogService.CloseAsync(this, result);
    }

    /// <summary>
    /// Completes the close after the container has run its close sequence.
    /// Called by the container when animation and unlock are done.
    /// </summary>
    internal void CompleteClose(DialogResult result)
    {
        _dialogService.CompleteClose(this, result);
    }
}
