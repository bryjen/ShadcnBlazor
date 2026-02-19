namespace ShadcnBlazor.Components.Shared.Models.Options;

/// <summary>
/// Event arguments passed to <see cref="IKeyInterceptorService"/> callbacks when a key is intercepted.
/// Matches the shape produced by the key-interceptor.js module.
/// </summary>
/// <remarks>
/// Components using <see cref="IKeyInterceptorService.ConnectAsync{T}"/> must implement
/// <c>[JSInvokable] OnKeyDown(string elementId, KeyInterceptorEventArgs args)</c> and optionally
/// <c>[JSInvokable] OnKeyUp(string elementId, KeyInterceptorEventArgs args)</c> to receive these events.
/// </remarks>
public record KeyInterceptorEventArgs(
    string Key,
    string Code,
    int Location,
    bool Repeat,
    bool CtrlKey,
    bool ShiftKey,
    bool AltKey,
    bool MetaKey,
    string? Type = null);
