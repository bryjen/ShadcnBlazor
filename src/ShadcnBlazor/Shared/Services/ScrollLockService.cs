using Microsoft.JSInterop;

namespace ShadcnBlazor.Shared.Services;

public class ScrollLockService(IJSRuntime jsRuntime) : IScrollLockService
{
    private int _lockCount;

    public async ValueTask LockAsync()
    {
        _lockCount++;
        if (_lockCount == 1)
        {
            await jsRuntime.InvokeVoidAsync("lockScroll");
        }
    }

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
