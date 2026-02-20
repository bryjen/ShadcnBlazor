using ShadcnBlazor.Docs.Services.Interop;

namespace ShadcnBlazor.Docs.Services;

/// <summary>
/// Theme service that manages the current theme state and applies updates via JS interop.
/// </summary>
public sealed class ThemeService
{
    private readonly ThemeInterop _themeInterop;

    /// <summary>
    /// Creates a new <see cref="ThemeService"/> instance.
    /// </summary>
    /// <param name="themeInterop">The theme JavaScript interop.</param>
    public ThemeService(ThemeInterop themeInterop)
    {
        _themeInterop = themeInterop;
        CurrentTheme = new ThemeState();
    }

    /// <summary>
    /// Gets the current theme state.
    /// </summary>
    public ThemeState CurrentTheme { get; private set; }

    /// <summary>
    /// Saves the provided theme by overriding all theme values in the UI.
    /// </summary>
    /// <param name="theme">Theme state to apply.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task SaveThemeAsync(ThemeState theme, CancellationToken cancellationToken = default)
    {
        CurrentTheme = theme.Clone();
        await _themeInterop.SetVarsAsync(CurrentTheme.ToCssVarMap(), cancellationToken);
    }

    /// <summary>
    /// Updates the primary color of the current theme.
    /// </summary>
    /// <param name="value">CSS color value.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task SetPrimaryAsync(string value, CancellationToken cancellationToken = default)
    {
        CurrentTheme.Primary = value;
        await _themeInterop.SetVarAsync("--primary", value, cancellationToken);
    }
}
