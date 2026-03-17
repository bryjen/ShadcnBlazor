using Microsoft.JSInterop;

namespace ShadcnBlazor.Components.Shared.Services.Interop;

/// <summary>
/// JavaScript interop for locking and unlocking body scroll (e.g. when a modal is open).
/// </summary>
public class ScrollLockInterop : IAsyncDisposable
{
    /// <summary>
    /// Default module paths used when none are provided.
    /// </summary>
    public static readonly string[] DefaultModulePaths =
    [
        "/_content/ShadcnBlazor/js/scroll-lock.js",
    ];

    private readonly IJSRuntime _jsRuntime;
    private readonly string[] _modulePaths;
    private IJSObjectReference? _module;
    private readonly SemaphoreSlim _lock = new(1, 1);

    /// <summary>
    /// Creates a new <see cref="ScrollLockInterop"/> instance.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime for interop calls.</param>
    /// <param name="modulePaths">Paths to try when loading the scroll-lock module (e.g. <c>/_content/ShadcnBlazor/js/scroll-lock.js</c>). Uses <see cref="DefaultModulePaths"/> if null or empty.</param>
    public ScrollLockInterop(IJSRuntime jsRuntime, string[]? modulePaths = null)
    {
        _jsRuntime = jsRuntime;
        _modulePaths = modulePaths is { Length: > 0 } ? modulePaths : DefaultModulePaths;
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
            foreach (var path in _modulePaths)
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

    /// <summary>Locks body scroll to prevent background scrolling.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async ValueTask LockAsync(CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("lockScroll", cancellationToken);
    }

    /// <summary>Unlocks body scroll.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async ValueTask UnlockAsync(CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("unlockScroll", cancellationToken);
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
