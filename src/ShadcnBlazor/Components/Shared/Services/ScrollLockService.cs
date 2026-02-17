using Microsoft.JSInterop;

namespace ShadcnBlazor.Components.Shared.Services;

/// <summary>
/// Service for locking body scroll with reference counting (supports nested modals).
/// </summary>
/// <param name="jsRuntime">The JavaScript runtime for scroll lock interop.</param>
public class ScrollLockService(IJSRuntime jsRuntime) : IScrollLockService
{
    private int _lockCount;

    /// <inheritdoc />
    public async ValueTask LockAsync()
    {
        _lockCount++;
        if (_lockCount == 1)
        {
            await jsRuntime.InvokeVoidAsync("lockScroll");
        }
    }

    /// <inheritdoc />
    public async ValueTask UnlockAsync()
    {
        if (_lockCount > 0)
        {
            _lockCount--;
            if (_lockCount == 0)
            {
                await jsRuntime.InvokeVoidAsync("unlockScroll");
            }
        }
    }

    /// <inheritdoc />
    public async ValueTask ToggleAsync()
    {
        if (_lockCount > 0)
        {
            await UnlockAsync();
        }
        else
        {
            await LockAsync();
        }
    }
}
