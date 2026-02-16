namespace SampleWasmProject.Components.Core.Shared.Services;

/// <summary>
/// High-level API for locking body scroll. Uses ref counting internally so multiple consumers (e.g. stacked dialogs) work correctly.
/// </summary>
public interface IScrollLockService
{
    /// <summary>
    /// Locks body scroll. Increments the lock count.
    /// </summary>
    ValueTask LockAsync();

    /// <summary>
    /// Unlocks body scroll. Decrements the lock count. No-op if already unlocked.
    /// </summary>
    ValueTask UnlockAsync();

    /// <summary>
    /// Toggles scroll lock: if locked, unlocks; otherwise locks.
    /// </summary>
    ValueTask ToggleAsync();
}
