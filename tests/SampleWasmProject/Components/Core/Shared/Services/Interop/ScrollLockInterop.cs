using Microsoft.JSInterop;

namespace SampleWasmProject.Components.Core.Shared.Services.Interop;

public class ScrollLockInterop : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _module;
    private readonly SemaphoreSlim _lock = new(1, 1);

    private static readonly string[] ModulePaths =
    [
        "_content/ShadcnBlazor/js/scroll-lock.js",
        "js/scroll-lock.js"
    ];

    public ScrollLockInterop(IJSRuntime jsRuntime)
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

            throw lastEx ?? new InvalidOperationException("Failed to load scroll-lock module.");
        }
        finally
        {
            _lock.Release();
        }
    }

    public async ValueTask LockAsync(CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("lockScroll", cancellationToken);
    }

    public async ValueTask UnlockAsync(CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("unlockScroll", cancellationToken);
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
