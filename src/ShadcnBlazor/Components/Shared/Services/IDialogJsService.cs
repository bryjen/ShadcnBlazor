using Microsoft.JSInterop;

namespace ShadcnBlazor.Components.Shared.Services;

/// <summary>
/// Service for dialog JavaScript interop (initialize, open, close, dispose, focus).
/// </summary>
public interface IDialogJsService
{
    /// <summary>Initializes the dialog and registers the .NET callback reference.</summary>
    ValueTask InitializeAsync<T>(string dialogId, DotNetObjectReference<T> dotNetRef, CancellationToken cancellationToken = default) where T : class;

    /// <summary>Opens the dialog and triggers the open animation.</summary>
    ValueTask OpenAsync(string dialogId, CancellationToken cancellationToken = default);

    /// <summary>Closes the dialog and triggers the close animation.</summary>
    ValueTask CloseAsync(string dialogId, CancellationToken cancellationToken = default);

    /// <summary>Disposes the dialog instance and cleans up event listeners.</summary>
    ValueTask DisposeDialogAsync(string dialogId, CancellationToken cancellationToken = default);

    /// <summary>Moves focus to the first focusable element inside the dialog.</summary>
    ValueTask FocusFirstInDialogAsync(string dialogId, CancellationToken cancellationToken = default);
}
