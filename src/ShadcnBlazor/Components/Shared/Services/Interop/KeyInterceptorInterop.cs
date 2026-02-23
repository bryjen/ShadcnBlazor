using Microsoft.JSInterop;
using ShadcnBlazor.Components.Shared.Models.Options;
using System.Diagnostics.CodeAnalysis;

namespace ShadcnBlazor.Components.Shared.Services.Interop;

/// <summary>
/// JavaScript interop for key interception (connect, disconnect, update key options).
/// </summary>
public class KeyInterceptorInterop : IAsyncDisposable
{
    /// <summary>
    /// Default module paths used when none are provided.
    /// </summary>
    public static readonly string[] DefaultModulePaths =
    [
        "/_content/ShadcnBlazor/js/key-interceptor.js",
    ];

    private readonly IJSRuntime _jsRuntime;
    private readonly string[] _modulePaths;
    private IJSObjectReference? _module;
    private readonly SemaphoreSlim _lock = new(1, 1);

    /// <summary>
    /// Creates a new <see cref="KeyInterceptorInterop"/> instance.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime for interop calls.</param>
    /// <param name="modulePaths">Paths to try when loading the key-interceptor module. Uses <see cref="DefaultModulePaths"/> if null or empty.</param>
    public KeyInterceptorInterop(IJSRuntime jsRuntime, string[]? modulePaths = null)
    {
        _jsRuntime = jsRuntime;
        _modulePaths = modulePaths is { Length: > 0 } ? modulePaths : DefaultModulePaths;
    }

    private async ValueTask<IJSObjectReference> GetModuleAsync(CancellationToken cancellationToken = default)
    {
        if (_module != null)
            return _module;

        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (_module != null)
                return _module;

            Exception? lastEx = null;
            foreach (var path in _modulePaths)
            {
                try
                {
                    _module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", cancellationToken, path);
                    return _module;
                }
                catch (JSException ex)
                {
                    lastEx = ex;
                }
            }

            throw lastEx ?? new InvalidOperationException("Failed to load key-interceptor module.");
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>Creates (or reuses) a key interceptor for an element and attaches handlers.</summary>
    public async ValueTask ConnectAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(DotNetObjectReference<T> dotNetObjectReference, string elementId, KeyInterceptorOptions options, CancellationToken cancellationToken = default) where T : class
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("connect", cancellationToken, dotNetObjectReference, elementId, options);
    }

    /// <summary>Detaches a key interceptor from an element.</summary>
    public async ValueTask DisconnectAsync(string elementId, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("disconnect", cancellationToken, elementId);
    }

    /// <summary>Updates the key option for an existing interceptor registration.</summary>
    public async ValueTask UpdateKeyAsync(string elementId, KeyOptions option, CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken);
        await module.InvokeVoidAsync("updatekey", cancellationToken, elementId, option);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_module != null)
        {
            await _module.DisposeAsync();
            _module = null;
        }
    }
}
