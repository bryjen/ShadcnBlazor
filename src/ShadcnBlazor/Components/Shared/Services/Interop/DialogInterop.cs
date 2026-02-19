using Microsoft.JSInterop;

namespace ShadcnBlazor.Components.Shared.Services.Interop;

/// <summary>
/// JavaScript interop for dialog open/close, focus management, and lifecycle.
/// </summary>
public class DialogInterop : IAsyncDisposable
{
    /// <summary>
    /// Default module paths used when none are provided.
    /// </summary>
    public static readonly string[] DefaultModulePaths =
    [
        "/ShadcnBlazor/_content/ShadcnBlazor/js/dialog.js",
    ];

    private readonly IJSRuntime _jsRuntime;
    private readonly string[] _modulePaths;
    private IJSObjectReference? _module;
    private readonly SemaphoreSlim _lock = new(1, 1);

    /// <summary>
    /// Creates a new <see cref="DialogInterop"/> instance.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime for interop calls.</param>
    /// <param name="modulePaths">Paths to try when loading the dialog module. Uses <see cref="DefaultModulePaths"/> if null or empty.</param>
    public DialogInterop(IJSRuntime jsRuntime, string[]? modulePaths = null)
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

            throw lastEx ?? new InvalidOperationException("Failed to load dialog module.");
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>Initializes the dialog and registers the .NET callback reference.</summary>
    public async ValueTask InitializeAsync<T>(string dialogId, DotNetObjectReference<T> dotNetRef, CancellationToken cancellationToken = default) where T : class
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("initialize", cancellationToken, dialogId, dotNetRef);
    }

    /// <summary>Opens the dialog and triggers the open animation.</summary>
    public async ValueTask OpenAsync(string dialogId, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("open", cancellationToken, dialogId);
    }

    /// <summary>Closes the dialog and triggers the close animation.</summary>
    public async ValueTask CloseAsync(string dialogId, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("close", cancellationToken, dialogId);
    }

    /// <summary>Disposes the dialog instance and cleans up event listeners.</summary>
    public async ValueTask DisposeDialogAsync(string dialogId, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("dispose", cancellationToken, dialogId);
    }

    /// <summary>Moves focus to the first focusable element inside the dialog.</summary>
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
