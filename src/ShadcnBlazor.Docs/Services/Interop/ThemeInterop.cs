using Microsoft.JSInterop;

namespace ShadcnBlazor.Docs.Services.Interop;

/// <summary>
/// JavaScript interop wrapper for theme updates.
/// </summary>
public sealed class ThemeInterop
{
    private readonly IJSRuntime _jsRuntime;

    /// <summary>
    /// Creates a new <see cref="ThemeInterop"/> instance.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime.</param>
    public ThemeInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Sets a single CSS variable on the root element.
    /// </summary>
    /// <param name="name">CSS variable name (e.g. "--primary").</param>
    /// <param name="value">CSS value (e.g. "#3B82F6" or "oklch(...)").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public ValueTask SetVarAsync(string name, string value, CancellationToken cancellationToken = default)
        => _jsRuntime.InvokeVoidAsync("shadcnTheme.setVar", cancellationToken, name, value);

    /// <summary>
    /// Sets multiple CSS variables on the root element.
    /// </summary>
    /// <param name="values">CSS variable name/value pairs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public ValueTask SetVarsAsync(IReadOnlyDictionary<string, string> values, CancellationToken cancellationToken = default)
        => _jsRuntime.InvokeVoidAsync("shadcnTheme.setVars", cancellationToken, values);

    /// <summary>
    /// Injects a stylesheet link if it is not already present in the document.
    /// </summary>
    public ValueTask InjectStylesheetAsync(string url, CancellationToken cancellationToken = default)
        => _jsRuntime.InvokeVoidAsync("shadcnTheme.injectStylesheet", cancellationToken, url);
}
