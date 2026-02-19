using Microsoft.JSInterop;

namespace ShadcnBlazor.Components.Popover.Services;

/// <summary>
/// JavaScript interop for popover positioning, outside-click handling, and lifecycle.
/// </summary>
public class PopoverInterop : IAsyncDisposable
{
    /// <summary>
    /// Default module paths used when none are provided.
    /// </summary>
    public static readonly string[] DefaultModulePaths =
    [
        "/ShadcnBlazor/_content/ShadcnBlazor/js/popovers.js",
    ];

    private readonly IJSRuntime _jsRuntime;
    private readonly string[] _modulePaths;
    private IJSObjectReference? _module;
    private readonly SemaphoreSlim _lock = new(1, 1);

    /// <summary>
    /// Creates a new <see cref="PopoverInterop"/> instance.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime for interop calls.</param>
    /// <param name="modulePaths">Paths to try when loading the popovers module. Uses <see cref="DefaultModulePaths"/> if null or empty.</param>
    public PopoverInterop(IJSRuntime jsRuntime, string[]? modulePaths = null)
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

            throw lastEx ?? new InvalidOperationException("Failed to load popovers module.");
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>Initializes the popover module with the given options.</summary>
    public async ValueTask InitializeAsync(string containerClass, int flipMargin, int overflowPadding, int baseZIndex, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("initialize", cancellationToken, containerClass, flipMargin, overflowPadding, baseZIndex);
    }

    /// <summary>Sets the debounce delay for repositioning.</summary>
    public async ValueTask SetRepositionDebounceAsync(int debounceMilliseconds, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("setRepositionDebounce", cancellationToken, debounceMilliseconds);
    }

    /// <summary>Connects an anchor to its popover for positioning.</summary>
    public async ValueTask ConnectAsync(string anchorId, string popoverId, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("connect", cancellationToken, anchorId, popoverId);
    }

    /// <summary>Disconnects a popover from its anchor.</summary>
    public async ValueTask DisconnectAsync(string popoverId, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("disconnect", cancellationToken, popoverId);
    }

    /// <summary>Enables close-on-outside-click for a popover.</summary>
    public async ValueTask EnableOutsideClickCloseAsync(string anchorId, string popoverId, DotNetObjectReference<Popover> callbackReference, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("enableOutsideClickClose", cancellationToken, anchorId, popoverId, callbackReference);
    }

    /// <summary>Disables close-on-outside-click for a popover.</summary>
    public async ValueTask DisableOutsideClickCloseAsync(string popoverId, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("disableOutsideClickClose", cancellationToken, popoverId);
    }

    /// <summary>Disposes the popover module resources.</summary>
    public async ValueTask DisposeAsync()
    {
        if (_module != null)
        {
            try
            {
                await _module.InvokeVoidAsync("dispose");
            }
            finally
            {
                await _module.DisposeAsync();
                _module = null;
            }
        }
    }
}
