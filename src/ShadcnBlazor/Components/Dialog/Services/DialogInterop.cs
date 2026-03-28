using Microsoft.JSInterop;

namespace ShadcnBlazor.Components.Dialog.Services;

/// <summary>
/// JavaScript interop for dialog focus management and event listeners.
/// </summary>
public class DialogInterop : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _module;
    private readonly SemaphoreSlim _lock = new(1, 1);

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogInterop"/> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime.</param>
    public DialogInterop(IJSRuntime jsRuntime)
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

            _module = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                "import",
                cancellationToken,
                "/_content/ShadcnBlazor/js/dialog.js"
            );
            return _module;
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>Initializes the dialog and registers event listeners.</summary>
    public async ValueTask InitializeAsync<T>(string dialogId, DotNetObjectReference<T> dotNetRef, CancellationToken cancellationToken = default) where T : class
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("initialize", cancellationToken, dialogId, dotNetRef);
    }

    /// <summary>Focuses the first focusable element in the dialog.</summary>
    public async ValueTask FocusFirstInDialogAsync(string dialogId, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("focusFirstInDialog", cancellationToken, dialogId);
    }

    /// <summary>Disposes the dialog and cleans up event listeners.</summary>
    public async ValueTask DisposeAsync(string dialogId, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("dispose", cancellationToken, dialogId);
    }

    /// <inheritdoc />
    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (_module != null)
        {
            await _module.DisposeAsync();
            _module = null;
        }
    }
}
