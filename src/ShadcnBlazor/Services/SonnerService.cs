using Microsoft.JSInterop;

namespace ShadcnBlazor.Services;

/// <summary>
/// Service for showing toast notifications using the Sonner library.
/// </summary>
/// <remarks>
/// Provides an async API for showing toast notifications through the window.Sonner JavaScript library.
/// The Sonner library must be loaded globally before Blazor initializes.
/// </remarks>
public class SonnerService
{
    private readonly IJSRuntime _jsRuntime;
    private int _callbackCounter = 0;
    private Dictionary<string, Func<ValueTask>> _callbacks = new();

    /// <summary>
    /// Creates a new SonnerService.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime.</param>
    public SonnerService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Initializes the Sonner service for callback support.
    /// Must be called once from a Blazor component during initialization.
    /// </summary>
    public async ValueTask InitializeAsync()
    {
        try
        {
            var reference = DotNetObjectReference.Create(this);
            await _jsRuntime.InvokeAsync<object>("InitializeSonnerCallbacks", reference);
        }
        catch (JSDisconnectedException)
        {
            // Expected during circuit shutdown.
        }
        catch (OperationCanceledException)
        {
            // Operation was cancelled.
        }
    }

    /// <summary>
    /// Invoked from JavaScript when a toast action button is clicked.
    /// </summary>
    /// <param name="callbackId">The ID of the callback to invoke.</param>
    [JSInvokable]
    public async Task InvokeCallback(string callbackId)
    {
        if (_callbacks.TryGetValue(callbackId, out var callback))
        {
            await callback();
            _callbacks.Remove(callbackId);
        }
    }

    private void RegisterCallback(string callbackId, Func<ValueTask> callback)
    {
        _callbacks[callbackId] = callback;
    }

    private string GenerateCallbackId()
    {
        return $"callback_{Guid.NewGuid().ToString().Substring(0, 8)}_{_callbackCounter++}";
    }

    /// <summary>
    /// Shows a generic toast notification.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="options">Optional configuration object for the toast.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A value task representing the asynchronous operation.</returns>
    public async ValueTask Show(string message, object? options = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await _jsRuntime.InvokeAsync<object>("window.Sonner.show", cancellationToken, message, options);
        }
        catch (JSDisconnectedException)
        {
            // Expected during circuit shutdown.
        }
        catch (OperationCanceledException)
        {
            // Operation was cancelled.
        }
    }

    /// <summary>
    /// Shows a success toast notification.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="options">Optional configuration object for the toast.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A value task representing the asynchronous operation.</returns>
    public async ValueTask Success(string message, object? options = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await _jsRuntime.InvokeAsync<object>("window.Sonner.success", cancellationToken, message, options);
        }
        catch (JSDisconnectedException)
        {
            // Expected during circuit shutdown.
        }
        catch (OperationCanceledException)
        {
            // Operation was cancelled.
        }
    }

    /// <summary>
    /// Shows an error toast notification.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="options">Optional configuration object for the toast.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A value task representing the asynchronous operation.</returns>
    public async ValueTask Error(string message, object? options = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await _jsRuntime.InvokeAsync<object>("window.Sonner.error", cancellationToken, message, options);
        }
        catch (JSDisconnectedException)
        {
            // Expected during circuit shutdown.
        }
        catch (OperationCanceledException)
        {
            // Operation was cancelled.
        }
    }

    /// <summary>
    /// Shows a warning toast notification.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="options">Optional configuration object for the toast.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A value task representing the asynchronous operation.</returns>
    public async ValueTask Warning(string message, object? options = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await _jsRuntime.InvokeAsync<object>("window.Sonner.warning", cancellationToken, message, options);
        }
        catch (JSDisconnectedException)
        {
            // Expected during circuit shutdown.
        }
        catch (OperationCanceledException)
        {
            // Operation was cancelled.
        }
    }

    /// <summary>
    /// Shows a promise-based toast notification that updates based on the promise state.
    /// </summary>
    /// <typeparam name="T">The type of the promise result.</typeparam>
    /// <param name="promise">The promise to track.</param>
    /// <param name="successMsg">Message or object to display on success.</param>
    /// <param name="errorMsg">Message or object to display on error.</param>
    /// <param name="options">Optional configuration object for the toast.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A value task representing the asynchronous operation.</returns>
    public async ValueTask Promise<T>(Task<T> promise, object successMsg, object errorMsg, object? options = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await _jsRuntime.InvokeAsync<object>("window.Sonner.promise", cancellationToken, promise, successMsg, errorMsg, options);
        }
        catch (JSDisconnectedException)
        {
            // Expected during circuit shutdown.
        }
        catch (OperationCanceledException)
        {
            // Operation was cancelled.
        }
    }

    /// <summary>
    /// Shows a success toast with an action button.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="actionLabel">The label for the action button.</param>
    /// <param name="onActionClick">Callback invoked when the action button is clicked.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A value task representing the asynchronous operation.</returns>
    public async ValueTask SuccessWithAction(string message, string actionLabel, Func<ValueTask> onActionClick, CancellationToken cancellationToken = default)
    {
        var callbackId = GenerateCallbackId();
        RegisterCallback(callbackId, onActionClick);

        try
        {
            var options = new { action = new { label = actionLabel, callbackId } };
            await _jsRuntime.InvokeAsync<object>("window.Sonner.success", cancellationToken, message, options);
        }
        catch (JSDisconnectedException)
        {
            // Expected during circuit shutdown.
        }
        catch (OperationCanceledException)
        {
            // Operation was cancelled.
        }
    }

    /// <summary>
    /// Shows an error toast with an action button.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="actionLabel">The label for the action button.</param>
    /// <param name="onActionClick">Callback invoked when the action button is clicked.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A value task representing the asynchronous operation.</returns>
    public async ValueTask ErrorWithAction(string message, string actionLabel, Func<ValueTask> onActionClick, CancellationToken cancellationToken = default)
    {
        var callbackId = GenerateCallbackId();
        RegisterCallback(callbackId, onActionClick);

        try
        {
            var options = new { action = new { label = actionLabel, callbackId } };
            await _jsRuntime.InvokeAsync<object>("window.Sonner.error", cancellationToken, message, options);
        }
        catch (JSDisconnectedException)
        {
            // Expected during circuit shutdown.
        }
        catch (OperationCanceledException)
        {
            // Operation was cancelled.
        }
    }

    /// <summary>
    /// Dismisses one or all toast notifications.
    /// </summary>
    /// <param name="toastId">Optional ID of a specific toast to dismiss. If null, dismisses all toasts.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A value task representing the asynchronous operation.</returns>
    public async ValueTask Dismiss(string? toastId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await _jsRuntime.InvokeAsync<object>("window.Sonner.dismiss", cancellationToken, toastId);
        }
        catch (JSDisconnectedException)
        {
            // Expected during circuit shutdown.
        }
        catch (OperationCanceledException)
        {
            // Operation was cancelled.
        }
    }

}

/// <summary>
/// Wrapper for C# callbacks to be invoked from JavaScript.
/// </summary>
internal class CallbackWrapper(Func<ValueTask> callback)
{
    [JSInvokable]
    public async Task Invoke()
    {
        await callback();
    }
}
