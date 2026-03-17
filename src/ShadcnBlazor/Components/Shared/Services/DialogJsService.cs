using Microsoft.JSInterop;
using ShadcnBlazor.Components.Shared.Services.Interop;

namespace ShadcnBlazor.Components.Shared.Services;

/// <summary>
/// Implementation of <see cref="IDialogJsService"/> for dialog JavaScript interop.
/// </summary>
public class DialogJsService : IDialogJsService
{
    private readonly DialogInterop _dialogInterop;

    /// <summary>
    /// Creates a new DialogJsService.
    /// </summary>
    /// <param name="dialogInterop">The dialog JavaScript interop.</param>
    public DialogJsService(DialogInterop dialogInterop)
    {
        _dialogInterop = dialogInterop;
    }

    /// <inheritdoc />
    public ValueTask InitializeAsync<T>(string dialogId, DotNetObjectReference<T> dotNetRef, CancellationToken cancellationToken = default) where T : class
        => _dialogInterop.InitializeAsync(dialogId, dotNetRef, cancellationToken);

    /// <inheritdoc />
    public ValueTask OpenAsync(string dialogId, CancellationToken cancellationToken = default)
        => _dialogInterop.OpenAsync(dialogId, cancellationToken);

    /// <inheritdoc />
    public ValueTask CloseAsync(string dialogId, CancellationToken cancellationToken = default)
        => _dialogInterop.CloseAsync(dialogId, cancellationToken);

    /// <inheritdoc />
    public ValueTask DisposeDialogAsync(string dialogId, CancellationToken cancellationToken = default)
        => _dialogInterop.DisposeDialogAsync(dialogId, cancellationToken);

    /// <inheritdoc />
    public ValueTask FocusFirstInDialogAsync(string dialogId, CancellationToken cancellationToken = default)
        => _dialogInterop.FocusFirstInDialogAsync(dialogId, cancellationToken);
}
