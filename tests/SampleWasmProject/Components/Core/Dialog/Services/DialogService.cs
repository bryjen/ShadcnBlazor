using Microsoft.AspNetCore.Components;
using SampleWasmProject.Components.Core.Dialog.Models;

namespace SampleWasmProject.Components.Core.Dialog.Services;

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
    /// </summary>
    /// <param name="reference">The dialog reference.</param>
    /// <param name="result">The result to return.</param>
    internal void Close(DialogReference reference, DialogResult result)
    {
        var instance = DialogInstances.FirstOrDefault(x => x.Id == reference.Id);
        if (instance == null)
        {
            return;
        }

        instance.TaskCompletionSource.TrySetResult(result);
        DialogInstances.Remove(instance);
        OnDialogsChanged?.Invoke();
    }
}
