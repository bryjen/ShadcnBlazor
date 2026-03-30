using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShadcnBlazor.Components.Shared.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShadcnBlazor.Components.Sonner.Services;

/// <summary>
/// Toast position on the screen.
/// </summary>
[JsonConverter(typeof(SonnerPositionConverter))]
public enum SonnerPosition
{
    TopLeft,
    TopCenter,
    TopRight,
    BottomLeft,
    BottomCenter,
    BottomRight
}

/// <summary>
/// JSON converter for SonnerPosition enum to kebab-case format.
/// </summary>
internal class SonnerPositionConverter : JsonConverter<SonnerPosition>
{
    public override SonnerPosition Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return value switch
        {
            "top-left" => SonnerPosition.TopLeft,
            "top-center" => SonnerPosition.TopCenter,
            "top-right" => SonnerPosition.TopRight,
            "bottom-left" => SonnerPosition.BottomLeft,
            "bottom-center" => SonnerPosition.BottomCenter,
            "bottom-right" => SonnerPosition.BottomRight,
            _ => throw new JsonException($"Unknown position: {value}")
        };
    }

    public override void Write(Utf8JsonWriter writer, SonnerPosition value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            SonnerPosition.TopLeft => "top-left",
            SonnerPosition.TopCenter => "top-center",
            SonnerPosition.TopRight => "top-right",
            SonnerPosition.BottomLeft => "bottom-left",
            SonnerPosition.BottomCenter => "bottom-center",
            SonnerPosition.BottomRight => "bottom-right",
            _ => throw new ArgumentException($"Unknown position: {value}")
        };
        writer.WriteStringValue(stringValue);
    }
}

/// <summary>
/// Configuration options for Sonner toast notifications.
/// </summary>
public record SonnerOptions(
    [property: JsonPropertyName("duration")]
    int? Duration = 4000,

    [property: JsonPropertyName("description")]
    string? Description = null,

    [property: JsonPropertyName("closeButton")]
    bool? CloseButton = false,

    [property: JsonPropertyName("className")]
    string? ClassName = null,

    [property: JsonPropertyName("style")]
    Dictionary<string, object>? Style = null,

    [property: JsonPropertyName("position")]
    SonnerPosition? Position = SonnerPosition.TopCenter
)
{
    /// <summary>
    /// Default options for all toasts (4 second duration, no close button, top center position).
    /// </summary>
    public static SonnerOptions Default { get; } = new();
}

/// <summary>
/// Internal class for toast action configuration with callback support.
/// </summary>
internal class SonnerOptionsWithAction
{
    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("closeButton")]
    public bool? CloseButton { get; set; }

    [JsonPropertyName("className")]
    public string? ClassName { get; set; }

    [JsonPropertyName("style")]
    public Dictionary<string, object>? Style { get; set; }

    [JsonPropertyName("position")]
    public SonnerPosition? Position { get; set; }

    [JsonPropertyName("action")]
    public SonnerActionOptions? Action { get; set; }
}

/// <summary>
/// Action button configuration for toasts.
/// </summary>
internal class SonnerActionOptions
{
    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("callbackId")]
    public string? CallbackId { get; set; }
}

/// <summary>
/// Service for showing toast notifications using the Sonner library.
/// </summary>
/// <remarks>
/// Provides an async API for showing toast notifications through the window.Sonner JavaScript library.
/// The Sonner library must be loaded globally before Blazor initializes.
/// </remarks>
[RegisterService]
public class SonnerService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly SonnerComponentRegistry _componentRegistry;
    private int _callbackCounter = 0;
    private Dictionary<string, Func<ValueTask>> _callbacks = new();
    private bool _initialized = false;
    private readonly object _initLock = new();

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
    /// Ensures the Sonner service is initialized for callback support.
    /// Called automatically on first use.
    /// </summary>
    private async ValueTask EnsureInitializedAsync()
    {
        if (_initialized)
            return;

        lock (_initLock)
        {
            if (_initialized)
                return;

            _initialized = true;
        }

        try
        {
            var reference = DotNetObjectReference.Create(this);
            await _jsRuntime.InvokeAsync<object>("InitializeSonnerCallbacks", reference);
        }
        catch (JSDisconnectedException)
        {
            // Expected during circuit shutdown.
            _initialized = false;
        }
        catch (OperationCanceledException)
        {
            // Operation was cancelled.
            _initialized = false;
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
    /// <param name="options">Optional configuration for the toast.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A value task representing the asynchronous operation.</returns>
    public async ValueTask Show(string message, SonnerOptions? options = null, CancellationToken cancellationToken = default) =>
        await ShowToastAsync("show", message, options, cancellationToken);

    /// <summary>
    /// Shows a success toast notification.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="options">Optional configuration for the toast.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A value task representing the asynchronous operation.</returns>
    public async ValueTask Success(string message, SonnerOptions? options = null, CancellationToken cancellationToken = default) =>
        await ShowToastAsync("success", message, options, cancellationToken);

    /// <summary>
    /// Shows an error toast notification.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="options">Optional configuration for the toast.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A value task representing the asynchronous operation.</returns>
    public async ValueTask Error(string message, SonnerOptions? options = null, CancellationToken cancellationToken = default) =>
        await ShowToastAsync("error", message, options, cancellationToken);

    /// <summary>
    /// Shows a warning toast notification.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="options">Optional configuration for the toast.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A value task representing the asynchronous operation.</returns>
    public async ValueTask Warning(string message, SonnerOptions? options = null, CancellationToken cancellationToken = default) =>
        await ShowToastAsync("warning", message, options, cancellationToken);

    /// <summary>
    /// Internal method to show basic toast notifications.
    /// </summary>
    private async ValueTask ShowToastAsync(string method, string message, SonnerOptions? options = null, CancellationToken cancellationToken = default)
    {
        await EnsureInitializedAsync();
        try
        {
            var serializedOptions = SerializeOptions(options);
            await _jsRuntime.InvokeAsync<object>($"window.Sonner.{method}", cancellationToken, message, serializedOptions);
        }
        catch (JSDisconnectedException) { }
        catch (OperationCanceledException) { }
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
            promiseId = await _jsRuntime.InvokeAsync<string>("window.Sonner.createPromiseToast", cancellationToken, loadingMsg, successMsg, errorMsg);
            await promise;

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
        catch (JSDisconnectedException) { }
        catch (OperationCanceledException)
        {
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
    /// <param name="options">Optional configuration for the toast.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A value task representing the asynchronous operation.</returns>
    public async ValueTask SuccessWithAction(string message, string actionLabel, Func<ValueTask> onActionClick, SonnerOptions? options = null, CancellationToken cancellationToken = default) =>
        await ShowToastWithActionAsync("success", message, actionLabel, onActionClick, options, cancellationToken);

    /// <summary>
    /// Shows an error toast with an action button.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="actionLabel">The label for the action button.</param>
    /// <param name="onActionClick">Callback invoked when the action button is clicked.</param>
    /// <param name="options">Optional configuration for the toast.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A value task representing the asynchronous operation.</returns>
    public async ValueTask ErrorWithAction(string message, string actionLabel, Func<ValueTask> onActionClick, SonnerOptions? options = null, CancellationToken cancellationToken = default) =>
        await ShowToastWithActionAsync("error", message, actionLabel, onActionClick, options, cancellationToken);

    /// <summary>
    /// Internal method to show toasts with action buttons.
    /// </summary>
    private async ValueTask ShowToastWithActionAsync(string method, string message, string actionLabel, Func<ValueTask> onActionClick, SonnerOptions? options = null, CancellationToken cancellationToken = default)
    {
        await EnsureInitializedAsync();
        var callbackId = GenerateCallbackId();
        RegisterCallback(callbackId, onActionClick);

        try
        {
            var optionsWithAction = BuildOptionsWithAction(options, actionLabel, callbackId);
            await _jsRuntime.InvokeAsync<object>($"window.Sonner.{method}", cancellationToken, message, optionsWithAction);
        }
        catch (JSDisconnectedException) { }
        catch (OperationCanceledException) { }
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
        catch (JSDisconnectedException) { }
        catch (OperationCanceledException) { }
    }

    /// <summary>
    /// Shows a headless toast that renders a RenderFragment.
    /// </summary>
    /// <param name="fragment">The RenderFragment to render as the entire toast content.</param>
    /// <param name="options">Optional configuration for the toast.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The toast ID as a string, or null if creation failed.</returns>
    public async ValueTask<string?> ShowHeadlessAsync(
        RenderFragment fragment,
        SonnerOptions? options = null,
        CancellationToken cancellationToken = default) =>
        await ShowComponentAsync("showComponent", fragment, options, cancellationToken);

    /// <summary>
    /// Shows a toast using Sonner's default structure, rendering a fragment in the title slot.
    /// </summary>
    /// <param name="fragment">The RenderFragment to render inside the toast.</param>
    /// <param name="options">Optional configuration for the toast.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The toast ID as a string, or null if creation failed.</returns>
    public async ValueTask<string?> ShowDefaultAsync(
        RenderFragment fragment,
        SonnerOptions? options = null,
        CancellationToken cancellationToken = default) =>
        await ShowComponentAsync("showComponentDefault", fragment, options, cancellationToken);

    /// <summary>
    /// Shows a toast with a custom Blazor component using proper Sonner styling (data-styled="true").
    /// Best for multi-toast scenarios with different heights - maintains proper hover/stacking behavior.
    /// </summary>
    /// <param name="fragment">The RenderFragment to render inside the toast.</param>
    /// <param name="options">Optional configuration for the toast.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The toast ID as a string, or null if creation failed.</returns>
    public async ValueTask<string?> ShowCustomAsync(
        RenderFragment fragment,
        SonnerOptions? options = null,
        CancellationToken cancellationToken = default) =>
        await ShowComponentAsync("showComponentStyled", fragment, options, cancellationToken);

    /// <summary>
    /// Internal method to show component-based toasts.
    /// </summary>
    private async ValueTask<string?> ShowComponentAsync(string method, RenderFragment fragment, SonnerOptions? options = null, CancellationToken cancellationToken = default)
    {
        var fragmentId = _componentRegistry.Register(fragment);
        var componentIdentifier = GetComponentIdentifier(typeof(SonnerComponentHost));

        try
        {
            var serializedOptions = SerializeOptions(options);
            var toastId = await _jsRuntime.InvokeAsync<string?>(
                $"window.Sonner.{method}",
                cancellationToken,
                componentIdentifier,
                fragmentId,
                serializedOptions);

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

    private static object? SerializeOptions(SonnerOptions? options)
    {
        var merged = options ?? SonnerOptions.Default;
        var json = JsonSerializer.Serialize(merged, GetSerializerOptions());
        return JsonSerializer.Deserialize<object>(json);
    }

    private static object? BuildOptionsWithAction(SonnerOptions? baseOptions, string actionLabel, string callbackId)
    {
        var opts = baseOptions ?? SonnerOptions.Default;
        var optionsWithAction = new SonnerOptionsWithAction
        {
            Duration = opts.Duration,
            Description = opts.Description,
            CloseButton = opts.CloseButton,
            ClassName = opts.ClassName,
            Style = opts.Style,
            Position = opts.Position,
            Action = new SonnerActionOptions { Label = actionLabel, CallbackId = callbackId }
        };

        var json = JsonSerializer.Serialize(optionsWithAction, GetSerializerOptions());
        return JsonSerializer.Deserialize<object>(json);
    }

    private static JsonSerializerOptions GetSerializerOptions() =>
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

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
