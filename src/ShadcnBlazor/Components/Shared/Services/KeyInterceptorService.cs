using Microsoft.JSInterop;
using ShadcnBlazor.Components.Shared.Models.Options;
using ShadcnBlazor.Components.Shared.Services.Interop;

namespace ShadcnBlazor.Components.Shared.Services;

/// <summary>
/// Implementation of <see cref="IKeyInterceptorService"/> for key interception JavaScript interop.
/// </summary>
public class KeyInterceptorService : IKeyInterceptorService
{
    private readonly KeyInterceptorInterop _keyInterceptorInterop;

    /// <summary>
    /// Creates a new KeyInterceptorService.
    /// </summary>
    /// <param name="keyInterceptorInterop">The key interceptor JavaScript interop.</param>
    public KeyInterceptorService(KeyInterceptorInterop keyInterceptorInterop)
    {
        _keyInterceptorInterop = keyInterceptorInterop;
    }

    /// <inheritdoc />
    public ValueTask ConnectAsync<T>(string elementId, DotNetObjectReference<T> dotNetRef, KeyInterceptorOptions options, CancellationToken cancellationToken = default) where T : class
        => _keyInterceptorInterop.ConnectAsync(dotNetRef, elementId, options, cancellationToken);

    /// <inheritdoc />
    public ValueTask DisconnectAsync(string elementId, CancellationToken cancellationToken = default)
        => _keyInterceptorInterop.DisconnectAsync(elementId, cancellationToken);

    /// <inheritdoc />
    public ValueTask UpdateKeyAsync(string elementId, KeyOptions option, CancellationToken cancellationToken = default)
        => _keyInterceptorInterop.UpdateKeyAsync(elementId, option, cancellationToken);
}
