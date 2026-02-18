using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Dialog.Models;

namespace ShadcnBlazor.Components.Dialog.Services;

/// <summary>
/// Implementation of <see cref="IDialogService"/> for imperative dialog display.
/// </summary>
public class DialogService : IDialogService
{
    /// <summary>
    /// List of currently open dialogs.
    /// </summary>
    internal List<DialogInstance> DialogInstances { get; } = [];

    /// <summary>
    /// Event raised when the dialog list changes (open/close).
    /// </summary>
    public event Action? OnDialogsChanged;

    /// <inheritdoc />
    public IDialogReference Show<T>(
        string title,
        DialogParameters? parameters = null,
        DialogOptions? options = null)
        where T : ComponentBase
    {
        var id = Guid.NewGuid();
        var dialogReference = new DialogReference(id, this);

        var dialogInstance = new DialogInstance
        {
            Id = id,
            Title = title,
            ComponentType = typeof(T),
            Parameters = parameters ?? new DialogParameters(),
            Options = options ?? new DialogOptions(),
            TaskCompletionSource = new TaskCompletionSource<DialogResult>(),
            Reference = dialogReference
        };

        DialogInstances.Add(dialogInstance);
        OnDialogsChanged?.Invoke();

        return dialogReference;
    }

    /// <inheritdoc />
    public async Task<IDialogReference> ShowAsync<T>(
        string title,
        DialogParameters? parameters = null,
        DialogOptions? options = null)
        where T : ComponentBase
    {
        return await Task.FromResult(Show<T>(title, parameters, options));
    }

    /// <summary>
    /// Closes the dialog with the specified reference and result.
    /// When a close handler is registered (by the container), invokes it first
    /// so animation and scroll unlock run before removal.
    /// </summary>
    /// <param name="reference">The dialog reference.</param>
    /// <param name="result">The result to return.</param>
    internal async Task CloseAsync(DialogReference reference, DialogResult result)
    {
        var instance = DialogInstances.FirstOrDefault(x => x.Id == reference.Id);
        if (instance == null)
        {
            return;
        }

        var handler = instance.CloseHandler;
        if (handler != null)
        {
            try
            {
                await handler(result);
            }
            catch
            {
                // Handler may throw; we still complete the close
            }
            // Handler calls CompleteClose when done; if it didn't (e.g. exception), remove now
            if (DialogInstances.Contains(instance))
            {
                CompleteClose(instance, result);
            }
            return;
        }

        CompleteClose(instance, result);
    }

    /// <summary>
    /// Completes the close by removing the instance and setting the result.
    /// Called by the container when its close sequence finishes, or when no handler is registered.
    /// </summary>
    internal void CompleteClose(DialogReference reference, DialogResult result)
    {
        var instance = DialogInstances.FirstOrDefault(x => x.Id == reference.Id);
        if (instance == null)
        {
            return;
        }

        CompleteClose(instance, result);
    }

    private void CompleteClose(DialogInstance instance, DialogResult result)
    {
        if (!DialogInstances.Remove(instance))
        {
            return;
        }

        instance.TaskCompletionSource.TrySetResult(result);
        OnDialogsChanged?.Invoke();
    }
}
