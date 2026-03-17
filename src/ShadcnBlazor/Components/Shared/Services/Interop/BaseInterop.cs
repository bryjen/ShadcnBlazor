using Microsoft.JSInterop;

namespace ShadcnBlazor.Components.Shared.Services.Interop;

/// <summary>
/// Abstract base class for JS Interop that manages module loading and lifecycle.
/// </summary>
public abstract class BaseInterop : IAsyncDisposable
{
    protected readonly IJSRuntime JSRuntime;
    private IJSObjectReference? _module;
    private readonly SemaphoreSlim _lock = new(1, 1);

    /// <summary>
    /// Gets the path to the JS module.
    /// </summary>
    protected abstract string GetModulePath();

    protected BaseInterop(IJSRuntime jsRuntime)
    {
        JSRuntime = jsRuntime;
    }

    /// <summary>
    /// Loads the JS module if not already loaded and returns a reference.
    /// </summary>
    protected async ValueTask<IJSObjectReference> GetModuleAsync(CancellationToken ct = default)
    {
        if (_module != null) return _module;

        await _lock.WaitAsync(ct);
        try
        {
            if (_module != null) return _module;
            
            var path = GetModulePath();
            
            // Ensure path starts with / or is a full _content path
            if (!path.StartsWith("/") && !path.Contains("_content/"))
            {
                path = "/" + path;
            }

            _module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", ct, path);
            return _module;
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_module != null)
        {
            try 
            { 
                await _module.DisposeAsync(); 
            } 
            catch (JSDisconnectedException) { }
            catch (OperationCanceledException) { }
            finally
            {
                _module = null;
            }
        }
        _lock.Dispose();
        GC.SuppressFinalize(this);
    }
}
