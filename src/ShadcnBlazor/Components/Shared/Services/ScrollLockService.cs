using ShadcnBlazor.Components.Shared.Services.Interop;

namespace ShadcnBlazor.Components.Shared.Services;

/// <summary>
/// Service for locking body scroll with reference counting (supports nested modals).
/// </summary>
/// <param name="scrollLockInterop">The JavaScript interop for scroll lock.</param>
public class ScrollLockService(ScrollLockInterop scrollLockInterop)
{
    private int _lockCount;

    public async ValueTask LockAsync()
    {
        _lockCount++;
        if (_lockCount == 1)
        {
            await scrollLockInterop.LockAsync();
        }
    }

    public async ValueTask UnlockAsync()
    {
        if (_lockCount > 0)
        {
            _lockCount--;
            if (_lockCount == 0)
            {
                await scrollLockInterop.UnlockAsync();
            }
        }
    }

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
