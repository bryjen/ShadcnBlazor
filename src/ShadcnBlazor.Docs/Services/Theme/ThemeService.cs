using ShadcnBlazor.Docs.Models.Theme;

namespace ShadcnBlazor.Docs.Services.Theme;

/// <summary>
/// Theme service that manages the current theme state and exposes runtime stylesheet output.
/// </summary>
public sealed class ThemeService
{
    /// <summary>
    /// Gets the current theme state.
    /// </summary>
    public ThemeStateFull CurrentTheme { get; private set; }

    /// <summary>
    /// Gets the current runtime stylesheet text injected by <c>App.razor</c>.
    /// </summary>
    public string RuntimeStyleSheet => ThemeStateFullConverter.ToStyleSheet(CurrentTheme);

    /// <summary>
    /// Raised when theme state changes and the app should re-render the runtime style block.
    /// </summary>
    public event Action? ThemeChanged;

    /// <summary>
    /// Gets the available themes.
    /// </summary>
    public IReadOnlyList<ThemePreset> Themes => _themes;

    /// <summary>
    /// Legacy alias for themes.
    /// </summary>
    public IReadOnlyList<ThemePreset> Presets => _themes;
    
    private readonly List<ThemePreset> _themes;
    private ThemeFetcher _themeFetcher;
    private bool _loadedExternalThemes;

    /// <summary>
    /// Creates a new <see cref="ThemeService"/> instance.
    /// </summary>
    public ThemeService(ThemeFetcher themeFetcher)
    {
        _themeFetcher = themeFetcher;
        
        CurrentTheme = CreateDefaultTheme();
        var themePreset = new ThemePreset("Default", ThemeFetcher.BuildSwatches(CurrentTheme), CurrentTheme.Clone());
        _themes = new();
        _themes.Add(themePreset);
    }

    public async Task EnsureExternalThemesLoadedAsync(CancellationToken cancellationToken = default)
    {
        if (_loadedExternalThemes)
            return;
        
        var tasks = ThemeFetcher.BuiltInThemes
            .Select(themeName => _themeFetcher.TryGetPresetAsync(themeName, cancellationToken))
            .ToArray();
        var presets = await Task.WhenAll(tasks);
        var filtered = presets
            .Where(preset => preset is not null)
            .Cast<ThemePreset>()
            .ToList();
        _themes.AddRange(filtered);

        _loadedExternalThemes = true;
    }
    
    /// <summary>
    /// Saves the provided theme by overriding all theme values in the UI.
    /// </summary>
    /// <param name="theme">Theme state to apply.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task SaveThemeAsync(ThemeStateFull theme, CancellationToken cancellationToken = default)
    {
        CurrentTheme = theme.Clone();
        ThemeChanged?.Invoke();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Applies a named preset by saving its complete theme values.
    /// </summary>
    /// <param name="preset">Preset to apply.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task ApplyPresetAsync(ThemePreset preset, CancellationToken cancellationToken = default)
    {
        await SaveThemeAsync(preset.Theme, cancellationToken);
    }

    private static ThemeStateFull CreateDefaultTheme()
    {
        var light = new ThemeState();
        var dark = new ThemeState
        {
            Background = "oklch(0.1448 0 0)",
            Foreground = "oklch(0.985 0 0)",
            Card = "oklch(0.21 0.006 285.885)",
            CardForeground = "oklch(0.985 0 0)",
            Popover = "oklch(0.21 0.006 285.885)",
            PopoverForeground = "oklch(0.985 0 0)",
            Primary = "oklch(0.488 0.243 264.376)",
            PrimaryForeground = "oklch(0.97 0.014 254.604)",
            Secondary = "oklch(0.274 0.006 286.033)",
            SecondaryForeground = "oklch(0.985 0 0)",
            Muted = "oklch(0.274 0.006 286.033)",
            MutedForeground = "oklch(0.705 0.015 286.067)",
            Accent = "oklch(0.274 0.006 286.033)",
            AccentForeground = "oklch(0.985 0 0)",
            Destructive = "oklch(0.704 0.191 22.216)",
            DestructiveForeground = "oklch(1 0 0)",
            Border = "oklch(1 0 0 / 0.1)",
            Input = "oklch(1 0 0 / 0.15)",
            Ring = "oklch(0.556 0 0)",
            Chart1 = "oklch(0.809 0.105 251.813)",
            Chart2 = "oklch(0.623 0.214 259.815)",
            Chart3 = "oklch(0.546 0.245 262.881)",
            Chart4 = "oklch(0.488 0.243 264.376)",
            Chart5 = "oklch(0.424 0.199 265.638)",
            Sidebar = "oklch(0.21 0.006 285.885)",
            SidebarForeground = "oklch(0.985 0 0)",
            SidebarPrimary = "oklch(0.623 0.214 259.815)",
            SidebarPrimaryForeground = "oklch(0.97 0.014 254.604)",
            SidebarAccent = "oklch(0.274 0.006 286.033)",
            SidebarAccentForeground = "oklch(0.985 0 0)",
            SidebarBorder = "oklch(1 0 0 / 0.1)",
            SidebarRing = "oklch(0.439 0 0)"
        };

        return new ThemeStateFull
        {
            Light = light,
            Dark = dark,
            Shared = light.Clone()
        };
    }
}
