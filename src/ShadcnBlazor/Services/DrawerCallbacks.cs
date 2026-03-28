using Microsoft.JSInterop;
using System.Threading;

namespace ShadcnBlazor.Services;

/// <summary>
/// Callbacks for Vaul drawer events.
/// </summary>
public sealed class DrawerCallbacks
{
    /// <summary>Called when the drawer open state changes.</summary>
    public Action<bool>? OnOpenChange { get; set; }
    /// <summary>Called when the drawer is closed.</summary>
    public Action? OnClose { get; set; }
    /// <summary>Called during dragging, providing the drag percentage.</summary>
    public Action<double>? OnDrag { get; set; }
    /// <summary>Called when the drawer is released.</summary>
    public Action<bool>? OnRelease { get; set; }
    /// <summary>Called when the drawer animation finishes.</summary>
    public Action<bool>? OnAnimationEnd { get; set; }
    /// <summary>Called when the active snap point changes.</summary>
    public Action<object?>? OnSnapPointChange { get; set; }
}

internal sealed class DrawerCallbackReceiver
{
    private readonly DrawerCallbacks? _callbacks;
    private readonly Func<string, Task> _release;
    private int _released;

    public DrawerCallbackReceiver(DrawerCallbacks? callbacks, Func<string, Task> release)
    {
        _callbacks = callbacks;
        _release = release;
    }

    public string? DrawerId { get; set; }

    [JSInvokable]
    public Task OnOpenChange(bool open)
    {
        _callbacks?.OnOpenChange?.Invoke(open);
        if (!open)
        {
            Release();
        }
        return Task.CompletedTask;
    }

    [JSInvokable]
    public Task OnClose()
    {
        _callbacks?.OnClose?.Invoke();
        Release();
        return Task.CompletedTask;
    }

    [JSInvokable]
    public Task OnDrag(double percentageDragged)
    {
        _callbacks?.OnDrag?.Invoke(percentageDragged);
        return Task.CompletedTask;
    }

    [JSInvokable]
    public Task OnRelease(bool open)
    {
        _callbacks?.OnRelease?.Invoke(open);
        return Task.CompletedTask;
    }

    [JSInvokable]
    public Task OnAnimationEnd(bool open)
    {
        _callbacks?.OnAnimationEnd?.Invoke(open);
        return Task.CompletedTask;
    }

    [JSInvokable]
    public Task OnSnapPointChange(object? value)
    {
        _callbacks?.OnSnapPointChange?.Invoke(value);
        return Task.CompletedTask;
    }

    [JSInvokable]
    public Task OnDisposed()
    {
        Release();
        return Task.CompletedTask;
    }

    private void Release()
    {
        var id = DrawerId;
        if (id is null)
        {
            return;
        }

        if (Interlocked.Exchange(ref _released, 1) == 1)
        {
            return;
        }

        _ = _release(id);
    }
}
