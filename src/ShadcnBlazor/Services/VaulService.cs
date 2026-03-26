using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShadcnBlazor.Components.Shared.Services;
using ShadcnBlazor.Components.Vaul;
using System.Collections.Concurrent;

namespace ShadcnBlazor.Services;

/// <summary>
/// Service for showing Vaul drawers using JavaScript interop.
/// </summary>
/// <remarks>
/// Provides an async API for opening Vaul drawers through the window.Vaul JavaScript library.
/// The Vaul interop bundle must be loaded globally before Blazor initializes.
/// </remarks>
public sealed class VaulService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly VaulComponentRegistry _componentRegistry;
    private readonly ScrollLockService _scrollLock;
    private readonly ConcurrentDictionary<string, DotNetObjectReference<VaulCallbackReceiver>> _callbackReferences = new();
    private readonly ConcurrentDictionary<string, byte> _lockedDrawers = new();

    /// <summary>
    /// Creates a new VaulService.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime.</param>
    /// <param name="componentRegistry">Registry for drawer fragments.</param>
    public VaulService(
        IJSRuntime jsRuntime,
        VaulComponentRegistry componentRegistry,
        ScrollLockService scrollLock)
    {
        _jsRuntime = jsRuntime;
        _componentRegistry = componentRegistry;
        _scrollLock = scrollLock;
    }

    /// <summary>
    /// Opens a drawer rendering a RenderFragment with typed options and callbacks.
    /// </summary>
    /// <param name="fragment">The RenderFragment to render inside the drawer.</param>
    /// <param name="options">Optional drawer options passed to Vaul.</param>
    /// <param name="callbacks">Optional callbacks for drawer events.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The drawer ID as a string, or null if creation failed.</returns>
    public async ValueTask<string?> OpenAsync(
        RenderFragment fragment,
        VaulOptions? options,
        VaulCallbacks? callbacks,
        CancellationToken cancellationToken = default)
    {
        var fragmentId = _componentRegistry.Register(fragment);
        var componentIdentifier = GetComponentIdentifier(typeof(VaulComponentHost));

        var receiver = new VaulCallbackReceiver(callbacks, ReleaseCallbacksAsync);
        var callbackReference = DotNetObjectReference.Create(receiver);

        try
        {
            var drawerId = await _jsRuntime.InvokeAsync<string?>(
                "window.Vaul.openComponent",
                cancellationToken,
                componentIdentifier,
                fragmentId,
                BuildOptions(options),
                callbackReference);

            if (drawerId is null)
            {
                _componentRegistry.Remove(fragmentId);
                callbackReference.Dispose();
                return null;
            }

            receiver.DrawerId = drawerId;
            _callbackReferences[drawerId] = callbackReference;
            await LockDrawerAsync(drawerId);

            return drawerId;
        }
        catch (JSDisconnectedException)
        {
            _componentRegistry.Remove(fragmentId);
            callbackReference.Dispose();
            return null;
        }
        catch (OperationCanceledException)
        {
            _componentRegistry.Remove(fragmentId);
            callbackReference.Dispose();
            return null;
        }
        catch (JSException)
        {
            _componentRegistry.Remove(fragmentId);
            callbackReference.Dispose();
            return null;
        }
    }

    /// <summary>
    /// Opens a drawer rendering a RenderFragment.
    /// </summary>
    /// <param name="fragment">The RenderFragment to render inside the drawer.</param>
    /// <param name="options">Optional drawer options passed to Vaul.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The drawer ID as a string, or null if creation failed.</returns>
    public async ValueTask<string?> OpenAsync(
        RenderFragment fragment,
        object? options = null,
        CancellationToken cancellationToken = default)
    {
        var fragmentId = _componentRegistry.Register(fragment);
        var componentIdentifier = GetComponentIdentifier(typeof(VaulComponentHost));

        try
        {
            var drawerId = await _jsRuntime.InvokeAsync<string?>(
                "window.Vaul.openComponent",
                cancellationToken,
                componentIdentifier,
                fragmentId,
                options);

            if (drawerId is null)
            {
                _componentRegistry.Remove(fragmentId);
            }

            if (drawerId is not null)
            {
                await LockDrawerAsync(drawerId);
            }

            return drawerId;
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
    /// Opens a drawer rendering a Blazor component with typed options and callbacks.
    /// </summary>
    /// <typeparam name="TComponent">The component type to render.</typeparam>
    /// <param name="parameters">Optional component parameters.</param>
    /// <param name="options">Optional drawer options passed to Vaul.</param>
    /// <param name="callbacks">Optional callbacks for drawer events.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The drawer ID as a string, or null if creation failed.</returns>
    public async ValueTask<string?> OpenAsync<TComponent>(
        Dictionary<string, object?>? parameters,
        VaulOptions? options,
        VaulCallbacks? callbacks,
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

        return await OpenAsync(fragment, options, callbacks, cancellationToken);
    }

    /// <summary>
    /// Opens a drawer rendering a Blazor component.
    /// </summary>
    /// <typeparam name="TComponent">The component type to render.</typeparam>
    /// <param name="parameters">Optional component parameters.</param>
    /// <param name="options">Optional drawer options passed to Vaul.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The drawer ID as a string, or null if creation failed.</returns>
    public async ValueTask<string?> OpenAsync<TComponent>(
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

        return await OpenAsync(fragment, options, cancellationToken);
    }

    /// <summary>
    /// Closes a drawer by ID.
    /// </summary>
    /// <param name="drawerId">The drawer ID returned from OpenAsync.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    public async ValueTask CloseAsync(string drawerId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _jsRuntime.InvokeAsync<object>("window.Vaul.close", cancellationToken, drawerId);
        }
        catch (JSDisconnectedException)
        {
            // Expected during circuit shutdown.
        }
        catch (OperationCanceledException)
        {
            // Operation was cancelled.
        }
        catch (JSException)
        {
            // JavaScript side not available.
        }

        await UnlockDrawerAsync(drawerId);
    }



    private async Task ReleaseCallbacksAsync(string drawerId)
    {
        if (_callbackReferences.TryRemove(drawerId, out var callbackReference))
        {
            callbackReference.Dispose();
        }

        await UnlockDrawerAsync(drawerId);
    }

    private async ValueTask LockDrawerAsync(string drawerId)
    {
        if (!_lockedDrawers.TryAdd(drawerId, 0))
        {
            return;
        }

        try
        {
            await _scrollLock.LockAsync();
        }
        catch (JSDisconnectedException)
        {
            _lockedDrawers.TryRemove(drawerId, out _);
        }
        catch (JSException)
        {
            _lockedDrawers.TryRemove(drawerId, out _);
        }
    }

    private async ValueTask UnlockDrawerAsync(string drawerId)
    {
        if (!_lockedDrawers.TryRemove(drawerId, out _))
        {
            return;
        }

        try
        {
            await _scrollLock.UnlockAsync();
        }
        catch (JSDisconnectedException)
        {
            // Ignore if JS is unavailable.
        }
        catch (JSException)
        {
            // Ignore if JS is unavailable.
        }
    }

    private static Dictionary<string, object?>? BuildOptions(VaulOptions? options)
    {
        if (options is null)
        {
            return null;
        }

        var result = new Dictionary<string, object?>();

        AddIfNotNull(result, "title", options.Title);
        AddIfNotNull(result, "description", options.Description);
        AddIfNotNull(result, "a11yHidden", options.A11yHidden);

        if (options.Drawer is not null)
        {
            var drawer = new Dictionary<string, object?>();
            AddIfNotNull(drawer, "dismissible", options.Drawer.Dismissible);
            AddIfNotNull(drawer, "modal", options.Drawer.Modal);
            AddIfNotNull(drawer, "direction", options.Drawer.Direction);
            AddIfNotNull(drawer, "closeThreshold", options.Drawer.CloseThreshold);
            AddIfNotNull(drawer, "scrollLockTimeout", options.Drawer.ScrollLockTimeout);
            AddIfNotNull(drawer, "handleOnly", options.Drawer.HandleOnly);
            AddIfNotNull(drawer, "fixed", options.Drawer.Fixed);
            AddIfNotNull(drawer, "nested", options.Drawer.Nested);
            AddIfNotNull(drawer, "repositionInputs", options.Drawer.RepositionInputs);
            AddIfNotNull(drawer, "preventScrollRestoration", options.Drawer.PreventScrollRestoration);
            AddIfNotNull(drawer, "disablePreventScroll", options.Drawer.DisablePreventScroll);
            AddIfNotNull(drawer, "autoFocus", options.Drawer.AutoFocus);
            AddIfNotNull(drawer, "shouldScaleBackground", options.Drawer.ShouldScaleBackground);
            AddIfNotNull(drawer, "setBackgroundColorOnScale", options.Drawer.SetBackgroundColorOnScale);
            AddIfNotNull(drawer, "backgroundColor", options.Drawer.BackgroundColor);
            AddIfNotNull(drawer, "noBodyStyles", options.Drawer.NoBodyStyles);
            AddIfNotNull(drawer, "fadeFromIndex", options.Drawer.FadeFromIndex);
            AddIfNotNull(drawer, "snapToSequentialPoint", options.Drawer.SnapToSequentialPoint);
            AddIfNotNull(drawer, "snapPoints", options.Drawer.SnapPoints);
            AddIfNotNull(drawer, "activeSnapPoint", options.Drawer.ActiveSnapPoint);
            AddIfNotNull(drawer, "additionalProps", options.Drawer.AdditionalProps);
            if (drawer.Count > 0)
            {
                result["drawer"] = drawer;
            }
        }

        if (options.Overlay is not null)
        {
            var overlay = new Dictionary<string, object?>();
            AddIfNotNull(overlay, "className", options.Overlay.ClassName);
            AddIfNotNull(overlay, "disableBaseClass", options.Overlay.DisableBaseClass);
            AddIfNotNull(overlay, "additionalProps", options.Overlay.AdditionalProps);
            if (overlay.Count > 0)
            {
                result["overlay"] = overlay;
            }
        }

        if (options.Content is not null)
        {
            var content = new Dictionary<string, object?>();
            AddIfNotNull(content, "className", options.Content.ClassName);
            AddIfNotNull(content, "disableBaseClass", options.Content.DisableBaseClass);
            AddIfNotNull(content, "additionalProps", options.Content.AdditionalProps);
            if (content.Count > 0)
            {
                result["content"] = content;
            }
        }

        return result.Count > 0 ? result : null;
    }

    private static void AddIfNotNull(
        IDictionary<string, object?> target,
        string key,
        object? value)
    {
        if (value is null)
        {
            return;
        }

        target[key] = value;
    }

    private static string GetComponentIdentifier(Type componentType)
    {
        var assemblyName = componentType.Assembly.GetName().Name ?? string.Empty;
        var typeName = componentType.FullName ?? componentType.Name;
        return $"{assemblyName}::{typeName}";
    }
}
