using Microsoft.JSInterop;

namespace ShadcnBlazor.Components.Dialog.Services;

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

    public async ValueTask InitializeAsync<T>(string dialogId, DotNetObjectReference<T> dotNetRef, CancellationToken cancellationToken = default) where T : class
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("initialize", cancellationToken, dialogId, dotNetRef);
    }

    public async ValueTask OpenAsync(string dialogId, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("open", cancellationToken, dialogId);
    }

    public async ValueTask CloseAsync(string dialogId, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("close", cancellationToken, dialogId);
    }

    public async ValueTask DisposeDialogAsync(string dialogId, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("dispose", cancellationToken, dialogId);
    }

    public async ValueTask FocusFirstInDialogAsync(string dialogId, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("focusFirstInDialog", cancellationToken, dialogId);
    }

    public async ValueTask DisposeAsync()
    {
        if (_module != null)
        {
            await _module.DisposeAsync();
            _module = null;
        }
    }
}
