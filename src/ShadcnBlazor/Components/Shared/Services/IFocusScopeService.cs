namespace ShadcnBlazor.Components.Shared.Services;

/// <summary>
/// Service for creating and managing focus scopes (trap focus within a container).
/// </summary>
public interface IFocusScopeService
{
    /// <summary>
    /// Creates a focus scope for the container element.
    /// </summary>
    /// <param name="containerId">ID of the container element.</param>
    /// <param name="loop">Whether Tab wraps from last to first element.</param>
    /// <param name="trapped">Whether focus is trapped within the container.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A handle that must be disposed (or UnmountAsync called) to clean up.</returns>
    ValueTask<IFocusScopeHandle> CreateFocusScopeAsync(string containerId, bool loop = false, bool trapped = false, CancellationToken cancellationToken = default);
}

/// <summary>
/// Handle for a focus scope; dispose or call UnmountAsync to clean up.
/// </summary>
public interface IFocusScopeHandle : IAsyncDisposable
{
    /// <summary>
    /// Unmounts the focus scope and restores focus.
    /// </summary>
    ValueTask UnmountAsync(CancellationToken cancellationToken = default);
}
