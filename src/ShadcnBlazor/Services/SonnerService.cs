using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShadcnBlazor.Components.Sonner;

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
    private readonly SonnerComponentRegistry _componentRegistry;
    private int _callbackCounter = 0;
    private Dictionary<string, Func<ValueTask>> _callbacks = new();

    /// <summary>
    /// Creates a new SonnerService.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime.</param>
    /// <param name="componentRegistry">Registry for component toast fragments.</param>
    public SonnerService(IJSRuntime jsRuntime, SonnerComponentRegistry componentRegistry)
    {
        _jsRuntime = jsRuntime;
        _componentRegistry = componentRegistry;
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
    /// Shows a promise-based toast notification that updates based on the task state.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="promise">The task to track.</param>
    /// <param name="loadingMsg">Message to display while loading.</param>
    /// <param name="successMsg">Message to display on success.</param>
    /// <param name="errorMsg">Message to display on error.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task PromiseAsync<T>(Task<T> promise, string loadingMsg, string successMsg, string errorMsg, CancellationToken cancellationToken = default)
    {
        string? promiseId = null;

        try
        {
            // Create the promise toast and get its ID
            promiseId = await _jsRuntime.InvokeAsync<string>("window.Sonner.createPromiseToast", cancellationToken, loadingMsg, successMsg, errorMsg);

            // Await the actual task
            await promise;

            // Resolve the promise toast on success
            if (promiseId != null)
            {
                try
                {
                    await _jsRuntime.InvokeAsync<object>("window.Sonner.resolvePromiseToast", cancellationToken, promiseId, "Success");
                }
                catch (JSDisconnectedException) { }
                catch (OperationCanceledException) { }
            }
        }
        catch (JSDisconnectedException)
        {
            // Expected during circuit shutdown.
        }
        catch (OperationCanceledException)
        {
            // Task was cancelled, reject the promise toast
            if (promiseId != null)
            {
                try
                {
                    await _jsRuntime.InvokeAsync<object>("window.Sonner.rejectPromiseToast", cancellationToken, promiseId, "Operation was cancelled");
                }
                catch (JSDisconnectedException) { }
                catch (OperationCanceledException) { }
            }
        }
        catch (Exception ex)
        {
            // Reject the promise toast on error
            if (promiseId != null)
            {
                try
                {
                    await _jsRuntime.InvokeAsync<object>("window.Sonner.rejectPromiseToast", cancellationToken, promiseId, ex.Message);
                }
                catch (JSDisconnectedException) { }
                catch (OperationCanceledException) { }
            }
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

    /// <summary>
    /// Shows a headless toast that renders a full Blazor component.
    /// </summary>
    /// <typeparam name="TComponent">The component type to render.</typeparam>
    /// <param name="parameters">Optional component parameters.</param>
    /// <param name="options">Optional toast options passed to Sonner.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The toast ID as a string, or null if creation failed.</returns>
    public async ValueTask<string?> ShowHeadlessAsync<TComponent>(
        Dictionary<string, object?>? parameters = null,
        object? options = null,
        CancellationToken cancellationToken = default)
        where TComponent : IComponent
    {
        RenderFragment fragment = builder =>
        {
            var seq = 0;
            builder.OpenComponent(seq++, typeof(TComponent));
            if (parameters != null)
            {
                foreach (var (key, value) in parameters)
                {
                    builder.AddAttribute(seq++, key, value);
                }
            }
            builder.CloseComponent();
        };

        return await ShowHeadlessAsync(fragment, options, cancellationToken);
    }

    /// <summary>
    /// Shows a headless toast that renders a RenderFragment.
    /// </summary>
    /// <param name="fragment">The RenderFragment to render inside the toast.</param>
    /// <param name="options">Optional toast options passed to Sonner.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The toast ID as a string, or null if creation failed.</returns>
    public async ValueTask<string?> ShowHeadlessAsync(
        RenderFragment fragment,
        object? options = null,
        CancellationToken cancellationToken = default)
    {
        var fragmentId = _componentRegistry.Register(fragment);
        var componentIdentifier = GetComponentIdentifier(typeof(SonnerComponentHost));

        try
        {
            var toastId = await _jsRuntime.InvokeAsync<string?>(
                "window.Sonner.showComponent",
                cancellationToken,
                componentIdentifier,
                fragmentId,
                options);

            if (toastId is null)
            {
                _componentRegistry.Remove(fragmentId);
            }

            return toastId;
        }
        catch (JSDisconnectedException)
        {
            _componentRegistry.Remove(fragmentId);
            return null;
        }
        catch (OperationCanceledException)
        {
            _componentRegistry.Remove(fragmentId);
            return null;
        }
        catch (JSException)
        {
            _componentRegistry.Remove(fragmentId);
            return null;
        }
    }

    /// <summary>
    /// Shows a toast using Sonner's default structure, rendering a component in the title slot.
    /// </summary>
    /// <typeparam name="TComponent">The component type to render.</typeparam>
    /// <param name="parameters">Optional component parameters.</param>
    /// <param name="options">Optional toast options passed to Sonner.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The toast ID as a string, or null if creation failed.</returns>
    public async ValueTask<string?> ShowDefaultAsync<TComponent>(
        Dictionary<string, object?>? parameters = null,
        object? options = null,
        CancellationToken cancellationToken = default)
        where TComponent : IComponent
    {
        RenderFragment fragment = builder =>
        {
            var seq = 0;
            builder.OpenComponent(seq++, typeof(TComponent));
            if (parameters != null)
            {
                foreach (var (key, value) in parameters)
                {
                    builder.AddAttribute(seq++, key, value);
                }
            }
            builder.CloseComponent();
        };

        return await ShowDefaultAsync(fragment, options, cancellationToken);
    }

    /// <summary>
    /// Shows a toast using Sonner's default structure, rendering a fragment in the title slot.
    /// </summary>
    /// <param name="fragment">The RenderFragment to render inside the toast title slot.</param>
    /// <param name="options">Optional toast options passed to Sonner.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The toast ID as a string, or null if creation failed.</returns>
    public async ValueTask<string?> ShowDefaultAsync(
        RenderFragment fragment,
        object? options = null,
        CancellationToken cancellationToken = default)
    {
        var fragmentId = _componentRegistry.Register(fragment);
        var componentIdentifier = GetComponentIdentifier(typeof(SonnerComponentHost));

        try
        {
            var toastId = await _jsRuntime.InvokeAsync<string?>(
                "window.Sonner.showComponentDefault",
                cancellationToken,
                componentIdentifier,
                fragmentId,
                options);

            if (toastId is null)
            {
                _componentRegistry.Remove(fragmentId);
            }

            return toastId;
        }
        catch (JSDisconnectedException)
        {
            _componentRegistry.Remove(fragmentId);
            return null;
        }
        catch (OperationCanceledException)
        {
            _componentRegistry.Remove(fragmentId);
            return null;
        }
        catch (JSException)
        {
            _componentRegistry.Remove(fragmentId);
            return null;
        }
    }

    private static string GetComponentIdentifier(Type componentType)
    {
        var assemblyName = componentType.Assembly.GetName().Name ?? string.Empty;
        var typeName = componentType.FullName ?? componentType.Name;
        return $"{assemblyName}::{typeName}";
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
