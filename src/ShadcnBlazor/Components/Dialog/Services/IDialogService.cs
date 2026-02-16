using Microsoft.AspNetCore.Components;

namespace ShadcnBlazor.Components.Dialog.Services;

/// <summary>
/// Service for showing dialogs imperatively.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Shows a dialog with the specified component type.
    /// </summary>
    /// <typeparam name="T">The component type to render as dialog content.</typeparam>
    /// <param name="title">The dialog title.</param>
    /// <param name="parameters">Optional parameters to pass to the component.</param>
    /// <param name="options">Optional dialog options.</param>
    /// <returns>A reference to the dialog.</returns>
    IDialogReference Show<T>(
        string title,
        Models.DialogParameters? parameters = null,
        Models.DialogOptions? options = null)
        where T : ComponentBase;

    /// <summary>
    /// Shows a dialog with the specified component type asynchronously.
    /// </summary>
    /// <typeparam name="T">The component type to render as dialog content.</typeparam>
    /// <param name="title">The dialog title.</param>
    /// <param name="parameters">Optional parameters to pass to the component.</param>
    /// <param name="options">Optional dialog options.</param>
    /// <returns>A task that completes with a reference to the dialog.</returns>
    Task<IDialogReference> ShowAsync<T>(
        string title,
        Models.DialogParameters? parameters = null,
        Models.DialogOptions? options = null)
        where T : ComponentBase;
}
