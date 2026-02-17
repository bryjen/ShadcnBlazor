using Microsoft.JSInterop;

namespace ShadcnBlazor.Components.Dialog.Services;

/// <summary>
/// JavaScript interop for dialog open/close, focus management, and lifecycle.
/// </summary>
public class DialogJsInterop : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _module;
    private readonly SemaphoreSlim _lock = new(1, 1);

    private static readonly string[] ModulePaths =
    [
        "_content/ShadcnBlazor/js/dialog.js",
        "js/dialog.js"
    ];

    /// <summary>
    /// Creates a new <see cref="DialogJsInterop"/> instance.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime for interop calls.</param>
    public DialogJsInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    private async ValueTask<IJSObjectReference> GetModuleAsync(CancellationToken cancellationToken = default)
    {
        if (_module != null)
            return _module;

        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (_module != null)
                return _module;

            Exception? lastEx = null;
            foreach (var path in ModulePaths)
            {
                try
                {
                    _module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", cancellationToken, path);
                    return _module;
                }
                catch (JSException ex)
                {
                    lastEx = ex;
                }
            }

            throw lastEx ?? new InvalidOperationException("Failed to load dialog module.");
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>
    /// Initializes the dialog in JavaScript and registers the .NET callback reference.
    /// </summary>
    /// <typeparam name="T">The type of the .NET object being referenced.</typeparam>
    /// <param name="dialogId">Unique identifier for the dialog.</param>
    /// <param name="dotNetRef">Reference to the .NET dialog component for callbacks.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async ValueTask InitializeAsync<T>(string dialogId, DotNetObjectReference<T> dotNetRef, CancellationToken cancellationToken = default) where T : class
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("initialize", cancellationToken, dialogId, dotNetRef);
    }

    /// <summary>Opens the dialog and triggers the open animation.</summary>
    /// <param name="dialogId">The dialog identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async ValueTask OpenAsync(string dialogId, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("open", cancellationToken, dialogId);
    }

    /// <summary>Closes the dialog and triggers the close animation.</summary>
    /// <param name="dialogId">The dialog identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async ValueTask CloseAsync(string dialogId, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("close", cancellationToken, dialogId);
    }

    /// <summary>Disposes the dialog instance in JavaScript and cleans up event listeners.</summary>
    /// <param name="dialogId">The dialog identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async ValueTask DisposeDialogAsync(string dialogId, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("dispose", cancellationToken, dialogId);
    }

    /// <summary>Moves focus to the first focusable element inside the dialog.</summary>
    /// <param name="dialogId">The dialog identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async ValueTask FocusFirstInDialogAsync(string dialogId, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("focusFirstInDialog", cancellationToken, dialogId);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_module != null)
        {
            await _module.DisposeAsync();
            _module = null;
        }
    }
}
