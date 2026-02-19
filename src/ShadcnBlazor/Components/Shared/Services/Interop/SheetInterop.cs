using Microsoft.JSInterop;

namespace ShadcnBlazor.Components.Shared.Services.Interop;

/// <summary>
/// JavaScript interop for sheet open/close, focus management, and lifecycle.
/// </summary>
public class SheetInterop : IAsyncDisposable
{
    /// <summary>
    /// Default module paths used when none are provided.
    /// </summary>
    public static readonly string[] DefaultModulePaths =
    [
        "/ShadcnBlazor/_content/ShadcnBlazor/js/sheet.js",
    ];

    private readonly IJSRuntime _jsRuntime;
    private readonly string[] _modulePaths;
    private IJSObjectReference? _module;
    private readonly SemaphoreSlim _lock = new(1, 1);

    /// <summary>
    /// Creates a new <see cref="SheetInterop"/> instance.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime for interop calls.</param>
    /// <param name="modulePaths">Paths to try when loading the sheet module. Uses <see cref="DefaultModulePaths"/> if null or empty.</param>
    public SheetInterop(IJSRuntime jsRuntime, string[]? modulePaths = null)
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

            throw lastEx ?? new InvalidOperationException("Failed to load sheet module.");
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>Initializes the sheet and registers the .NET callback reference.</summary>
    public async ValueTask InitializeAsync<T>(string sheetId, DotNetObjectReference<T> dotNetRef, CancellationToken cancellationToken = default) where T : class
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("initialize", cancellationToken, sheetId, dotNetRef);
    }

    /// <summary>Opens the sheet and triggers the open animation.</summary>
    public async ValueTask OpenAsync(string sheetId, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("open", cancellationToken, sheetId);
    }

    /// <summary>Closes the sheet and triggers the close animation.</summary>
    public async ValueTask CloseAsync(string sheetId, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("close", cancellationToken, sheetId);
    }

    /// <summary>Disposes the sheet instance and cleans up event listeners.</summary>
    public async ValueTask DisposeSheetAsync(string sheetId, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("dispose", cancellationToken, sheetId);
    }

    /// <summary>Moves focus to the first focusable element inside the sheet.</summary>
    public async ValueTask FocusFirstInSheetAsync(string sheetId, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("focusFirstInSheet", cancellationToken, sheetId);
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
