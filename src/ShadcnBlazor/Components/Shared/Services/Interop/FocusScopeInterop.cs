using Microsoft.JSInterop;

namespace ShadcnBlazor.Components.Shared.Services.Interop;

/// <summary>
/// JavaScript interop for focus scope (trap focus within a container, optional tab looping).
/// </summary>
public class FocusScopeInterop : IAsyncDisposable
{
    /// <summary>
    /// Default module paths used when none are provided.
    /// </summary>
    public static readonly string[] DefaultModulePaths =
    [
        "/ShadcnBlazor/_content/ShadcnBlazor/js/focus-scope.js",
    ];

    private readonly IJSRuntime _jsRuntime;
    private readonly string[] _modulePaths;
    private IJSObjectReference? _module;
    private readonly SemaphoreSlim _lock = new(1, 1);

    /// <summary>
    /// Creates a new <see cref="FocusScopeInterop"/> instance.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime for interop calls.</param>
    /// <param name="modulePaths">Paths to try when loading the focus-scope module. Uses <see cref="DefaultModulePaths"/> if null or empty.</param>
    public FocusScopeInterop(IJSRuntime jsRuntime, string[]? modulePaths = null)
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

            throw lastEx ?? new InvalidOperationException("Failed to load focus-scope module.");
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>
    /// Creates a focus scope for the container element.
    /// </summary>
    /// <param name="containerId">ID of the container element.</param>
    /// <param name="loop">Whether Tab wraps from last to first element.</param>
    /// <param name="trapped">Whether focus is trapped within the container.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Reference to the scope (call UnmountAsync to clean up).</returns>
    public async ValueTask<IJSObjectReference> CreateFocusScopeAsync(string containerId, bool loop = false, bool trapped = false, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        return await module.InvokeAsync<IJSObjectReference>("createFocusScopeById", cancellationToken, containerId, loop, trapped);
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
