using Microsoft.JSInterop;
using ShadcnBlazor.Components.Shared.Services.Interop;

namespace ShadcnBlazor.Components.Shared.Services;

/// <summary>
/// Implementation of <see cref="IFocusScopeService"/> for focus scope JavaScript interop.
/// </summary>
public class FocusScopeService : IFocusScopeService
{
    private readonly FocusScopeInterop _focusScopeInterop;

    /// <summary>
    /// Creates a new FocusScopeService.
    /// </summary>
    /// <param name="focusScopeInterop">The focus scope JavaScript interop.</param>
    public FocusScopeService(FocusScopeInterop focusScopeInterop)
    {
        _focusScopeInterop = focusScopeInterop;
    }

    /// <inheritdoc />
    public async ValueTask<IFocusScopeHandle> CreateFocusScopeAsync(string containerId, bool loop = false, bool trapped = false, CancellationToken cancellationToken = default)
    {
        var scopeRef = await _focusScopeInterop.CreateFocusScopeAsync(containerId, loop, trapped, cancellationToken);
        return new FocusScopeHandle(scopeRef);
    }
}

/// <summary>
/// Handle for a focus scope created by <see cref="FocusScopeService"/>.
/// </summary>
public sealed class FocusScopeHandle : IFocusScopeHandle
{
    private IJSObjectReference? _scopeRef;

    internal FocusScopeHandle(IJSObjectReference scopeRef)
    {
        _scopeRef = scopeRef;
    }

    /// <inheritdoc />
    public async ValueTask UnmountAsync(CancellationToken cancellationToken = default)
    {
        if (_scopeRef == null) return;

        try
        {
            await _scopeRef.InvokeVoidAsync("unmount", cancellationToken);
        }
        finally
        {
            await _scopeRef.DisposeAsync();
            _scopeRef = null;
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await UnmountAsync();
    }
}
