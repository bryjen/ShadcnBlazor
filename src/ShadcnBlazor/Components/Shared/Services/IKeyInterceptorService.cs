using Microsoft.JSInterop;
using ShadcnBlazor.Components.Shared.Models.Options;

namespace ShadcnBlazor.Components.Shared.Services;

/// <summary>
/// Service for key interception (connect, disconnect, update key options).
/// </summary>
/// <remarks>
/// The target type passed to <see cref="ConnectAsync{T}"/> must implement <c>[JSInvokable] OnKeyDown(string elementId, KeyInterceptorEventArgs args)</c>
/// and optionally <c>[JSInvokable] OnKeyUp(string elementId, KeyInterceptorEventArgs args)</c> to receive intercepted key events.
/// Use <see cref="KeyInterceptorEventArgs"/> for the callback parameter type.
/// </remarks>
public interface IKeyInterceptorService
{
    /// <summary>Creates (or reuses) a key interceptor for an element and attaches handlers.</summary>
    ValueTask ConnectAsync<T>(string elementId, DotNetObjectReference<T> dotNetRef, KeyInterceptorOptions options, CancellationToken cancellationToken = default) where T : class;

    /// <summary>Detaches a key interceptor from an element.</summary>
    ValueTask DisconnectAsync(string elementId, CancellationToken cancellationToken = default);

    /// <summary>Updates the key option for an existing interceptor registration.</summary>
    ValueTask UpdateKeyAsync(string elementId, KeyOptions option, CancellationToken cancellationToken = default);
}
