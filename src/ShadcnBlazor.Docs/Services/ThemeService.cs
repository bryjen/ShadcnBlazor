namespace ShadcnBlazor.Docs.Services;

/// <summary>
/// Theme service that manages the current theme state and exposes runtime stylesheet output.
/// </summary>
public sealed class ThemeService
{
    private readonly List<ThemePreset> _themes;

    /// <summary>
    /// Creates a new <see cref="ThemeService"/> instance.
    /// </summary>
    public ThemeService()
    {
        CurrentTheme = CreateDefaultTheme();

        _themes =
        [
            new ThemePreset("Default", BuildSwatches(CurrentTheme), CurrentTheme.Clone()),
            CreateAmethystHazePreset(),
            CreateBoldTechPreset(),
            CreateClaymorphismPreset(),
            CreateModernMinimalPreset()
        ];
    }

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

    /// <summary>
    /// Updates the primary color of the current theme.
    /// </summary>
    /// <param name="value">CSS color value.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task SetPrimaryAsync(string value, CancellationToken cancellationToken = default)
    {
        CurrentTheme.Dark.Primary = value;
        ThemeChanged?.Invoke();
        return Task.CompletedTask;
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
    private static string[] BuildSwatches(ThemeStateFull theme) =>
    [
        theme.Dark.Primary,
        theme.Dark.Secondary,
        theme.Dark.Accent,
        theme.Dark.Background
    ];

    private static ThemePreset CreateAmethystHazePreset() => new(
        "Amethyst Haze",
        ["oklch(0.7058 0.0777 302.0489)", "oklch(0.4604 0.0472 295.5578)", "oklch(0.3181 0.0321 308.6149)", "oklch(0.8391 0.0692 2.6681)"],
        new ThemeState
        {
            Radius = "0.5rem",
            Background = "oklch(0.2166 0.0215 292.8474)",
            Foreground = "oklch(0.9053 0.0245 293.5570)",
            Card = "oklch(0.2544 0.0301 292.7315)",
            CardForeground = "oklch(0.9053 0.0245 293.5570)",
            Popover = "oklch(0.2544 0.0301 292.7315)",
            PopoverForeground = "oklch(0.9053 0.0245 293.5570)",
            Primary = "oklch(0.7058 0.0777 302.0489)",
            PrimaryForeground = "oklch(0.2166 0.0215 292.8474)",
            Secondary = "oklch(0.4604 0.0472 295.5578)",
            SecondaryForeground = "oklch(0.9053 0.0245 293.5570)",
            Muted = "oklch(0.2560 0.0320 294.8380)",
            MutedForeground = "oklch(0.6974 0.0282 300.0614)",
            Accent = "oklch(0.3181 0.0321 308.6149)",
            AccentForeground = "oklch(0.8391 0.0692 2.6681)",
            Destructive = "oklch(0.6875 0.1420 21.4566)",
            DestructiveForeground = "oklch(0.2166 0.0215 292.8474)",
            Border = "oklch(0.3063 0.0359 293.3367)",
            Input = "oklch(0.2847 0.0346 291.2726)",
            Ring = "oklch(0.7058 0.0777 302.0489)",
            Chart1 = "oklch(0.7058 0.0777 302.0489)",
            Chart2 = "oklch(0.8391 0.0692 2.6681)",
            Chart3 = "oklch(0.7321 0.0749 169.8670)",
            Chart4 = "oklch(0.8540 0.0882 76.8292)",
            Chart5 = "oklch(0.7857 0.0645 258.0839)",
            Sidebar = "oklch(0.1985 0.0200 293.6639)",
            SidebarForeground = "oklch(0.9053 0.0245 293.5570)",
            SidebarPrimary = "oklch(0.7058 0.0777 302.0489)",
            SidebarPrimaryForeground = "oklch(0.2166 0.0215 292.8474)",
            SidebarAccent = "oklch(0.3181 0.0321 308.6149)",
            SidebarAccentForeground = "oklch(0.8391 0.0692 2.6681)",
            SidebarBorder = "oklch(0.2847 0.0346 291.2726)",
            SidebarRing = "oklch(0.7058 0.0777 302.0489)",
            FontSans = "Geist, sans-serif",
            FontSerif = "\"Lora\", Georgia, serif",
            FontMono = "\"Fira Code\", \"Courier New\", monospace",
            TrackingNormal = "0em",
            Spacing = "0.25rem",
            ShadowColor = "hsl(0 0% 0%)",
            ShadowOpacity = "0.06",
            ShadowBlur = "5px",
            ShadowSpread = "1px",
            ShadowX = "1px",
            ShadowY = "2px",
            Shadow2xs = "1px 2px 5px 1px hsl(0 0% 0% / 0.03)",
            ShadowXs = "1px 2px 5px 1px hsl(0 0% 0% / 0.03)",
            ShadowSm = "1px 2px 5px 1px hsl(0 0% 0% / 0.06), 1px 1px 2px 0px hsl(0 0% 0% / 0.06)",
            Shadow = "1px 2px 5px 1px hsl(0 0% 0% / 0.06), 1px 1px 2px 0px hsl(0 0% 0% / 0.06)",
            ShadowMd = "1px 2px 5px 1px hsl(0 0% 0% / 0.06), 1px 2px 4px 0px hsl(0 0% 0% / 0.06)",
            ShadowLg = "1px 2px 5px 1px hsl(0 0% 0% / 0.06), 1px 4px 6px 0px hsl(0 0% 0% / 0.06)",
            ShadowXl = "1px 2px 5px 1px hsl(0 0% 0% / 0.06), 1px 8px 10px 0px hsl(0 0% 0% / 0.06)",
            Shadow2xl = "1px 2px 5px 1px hsl(0 0% 0% / 0.15)"
        });

    private static ThemePreset CreateBoldTechPreset() => new(
        "Bold Tech",
        ["oklch(0.6056 0.2189 292.7172)", "oklch(0.4568 0.2146 277.0229)", "oklch(0.2827 0.1351 291.0894)", "oklch(0.2077 0.0398 265.7549)"],
        new ThemeState
        {
            Radius = "0.625rem",
            Background = "oklch(0.2077 0.0398 265.7549)",
            Foreground = "oklch(0.9299 0.0334 272.7879)",
            Card = "oklch(0.2573 0.0861 281.2883)",
            CardForeground = "oklch(0.9299 0.0334 272.7879)",
            Popover = "oklch(0.2573 0.0861 281.2883)",
            PopoverForeground = "oklch(0.9299 0.0334 272.7879)",
            Primary = "oklch(0.6056 0.2189 292.7172)",
            PrimaryForeground = "oklch(1.0000 0 0)",
            Secondary = "oklch(0.2573 0.0861 281.2883)",
            SecondaryForeground = "oklch(0.9299 0.0334 272.7879)",
            Muted = "oklch(0.2329 0.0919 279.1398)",
            MutedForeground = "oklch(0.8112 0.1013 293.5712)",
            Accent = "oklch(0.4568 0.2146 277.0229)",
            AccentForeground = "oklch(0.9299 0.0334 272.7879)",
            Destructive = "oklch(0.6368 0.2078 25.3313)",
            DestructiveForeground = "oklch(1.0000 0 0)",
            Border = "oklch(0.2827 0.1351 291.0894)",
            Input = "oklch(0.2827 0.1351 291.0894)",
            Ring = "oklch(0.6056 0.2189 292.7172)",
            Chart1 = "oklch(0.7090 0.1592 293.5412)",
            Chart2 = "oklch(0.6056 0.2189 292.7172)",
            Chart3 = "oklch(0.5413 0.2466 293.0090)",
            Chart4 = "oklch(0.4907 0.2412 292.5809)",
            Chart5 = "oklch(0.4320 0.2106 292.7591)",
            Sidebar = "oklch(0.2077 0.0398 265.7549)",
            SidebarForeground = "oklch(0.9299 0.0334 272.7879)",
            SidebarPrimary = "oklch(0.6056 0.2189 292.7172)",
            SidebarPrimaryForeground = "oklch(1.0000 0 0)",
            SidebarAccent = "oklch(0.4568 0.2146 277.0229)",
            SidebarAccentForeground = "oklch(0.9299 0.0334 272.7879)",
            SidebarBorder = "oklch(0.2827 0.1351 291.0894)",
            SidebarRing = "oklch(0.6056 0.2189 292.7172)",
            FontSans = "Roboto, sans-serif",
            FontSerif = "Playfair Display, serif",
            FontMono = "Fira Code, monospace",
            TrackingNormal = "0em",
            Spacing = "0.25rem",
            ShadowColor = "hsl(255 86% 66%)",
            ShadowOpacity = "0.2",
            ShadowBlur = "4px",
            ShadowSpread = "0px",
            ShadowX = "2px",
            ShadowY = "2px",
            Shadow2xs = "2px 2px 4px 0px hsl(255 86% 66% / 0.10)",
            ShadowXs = "2px 2px 4px 0px hsl(255 86% 66% / 0.10)",
            ShadowSm = "2px 2px 4px 0px hsl(255 86% 66% / 0.20), 2px 1px 2px -1px hsl(255 86% 66% / 0.20)",
            Shadow = "2px 2px 4px 0px hsl(255 86% 66% / 0.20), 2px 1px 2px -1px hsl(255 86% 66% / 0.20)",
            ShadowMd = "2px 2px 4px 0px hsl(255 86% 66% / 0.20), 2px 2px 4px -1px hsl(255 86% 66% / 0.20)",
            ShadowLg = "2px 2px 4px 0px hsl(255 86% 66% / 0.20), 2px 4px 6px -1px hsl(255 86% 66% / 0.20)",
            ShadowXl = "2px 2px 4px 0px hsl(255 86% 66% / 0.20), 2px 8px 10px -1px hsl(255 86% 66% / 0.20)",
            Shadow2xl = "2px 2px 4px 0px hsl(255 86% 66% / 0.50)"
        });

    private static ThemePreset CreateClaymorphismPreset() => new(
        "Claymorphism",
        ["oklch(0.6801 0.1583 276.9349)", "oklch(0.3896 0.0074 59.4734)", "oklch(0.3359 0.0077 59.4197)", "oklch(0.2244 0.0074 67.4370)"],
        new ThemeState
        {
            Radius = "1.25rem",
            Background = "oklch(0.2244 0.0074 67.4370)",
            Foreground = "oklch(0.9288 0.0126 255.5078)",
            Card = "oklch(0.2801 0.0080 59.3379)",
            CardForeground = "oklch(0.9288 0.0126 255.5078)",
            Popover = "oklch(0.2801 0.0080 59.3379)",
            PopoverForeground = "oklch(0.9288 0.0126 255.5078)",
            Primary = "oklch(0.6801 0.1583 276.9349)",
            PrimaryForeground = "oklch(0.2244 0.0074 67.4370)",
            Secondary = "oklch(0.3359 0.0077 59.4197)",
            SecondaryForeground = "oklch(0.8717 0.0093 258.3382)",
            Muted = "oklch(0.2287 0.0074 67.4469)",
            MutedForeground = "oklch(0.7137 0.0192 261.3246)",
            Accent = "oklch(0.3896 0.0074 59.4734)",
            AccentForeground = "oklch(0.8717 0.0093 258.3382)",
            Destructive = "oklch(0.6368 0.2078 25.3313)",
            DestructiveForeground = "oklch(0.2244 0.0074 67.4370)",
            Border = "oklch(0.3359 0.0077 59.4197)",
            Input = "oklch(0.3359 0.0077 59.4197)",
            Ring = "oklch(0.6801 0.1583 276.9349)",
            Chart1 = "oklch(0.6801 0.1583 276.9349)",
            Chart2 = "oklch(0.5854 0.2041 277.1173)",
            Chart3 = "oklch(0.5106 0.2301 276.9656)",
            Chart4 = "oklch(0.4568 0.2146 277.0229)",
            Chart5 = "oklch(0.3984 0.1773 277.3662)",
            Sidebar = "oklch(0.3359 0.0077 59.4197)",
            SidebarForeground = "oklch(0.9288 0.0126 255.5078)",
            SidebarPrimary = "oklch(0.6801 0.1583 276.9349)",
            SidebarPrimaryForeground = "oklch(0.2244 0.0074 67.4370)",
            SidebarAccent = "oklch(0.3896 0.0074 59.4734)",
            SidebarAccentForeground = "oklch(0.8717 0.0093 258.3382)",
            SidebarBorder = "oklch(0.3359 0.0077 59.4197)",
            SidebarRing = "oklch(0.6801 0.1583 276.9349)",
            FontSans = "Plus Jakarta Sans, sans-serif",
            FontSerif = "Lora, serif",
            FontMono = "Roboto Mono, monospace",
            TrackingNormal = "0em",
            Spacing = "0.25rem",
            ShadowColor = "hsl(0 0% 0%)",
            ShadowOpacity = "0.18",
            ShadowBlur = "10px",
            ShadowSpread = "4px",
            ShadowX = "2px",
            ShadowY = "2px",
            Shadow2xs = "2px 2px 10px 4px hsl(0 0% 0% / 0.09)",
            ShadowXs = "2px 2px 10px 4px hsl(0 0% 0% / 0.09)",
            ShadowSm = "2px 2px 10px 4px hsl(0 0% 0% / 0.18), 2px 1px 2px 3px hsl(0 0% 0% / 0.18)",
            Shadow = "2px 2px 10px 4px hsl(0 0% 0% / 0.18), 2px 1px 2px 3px hsl(0 0% 0% / 0.18)",
            ShadowMd = "2px 2px 10px 4px hsl(0 0% 0% / 0.18), 2px 2px 4px 3px hsl(0 0% 0% / 0.18)",
            ShadowLg = "2px 2px 10px 4px hsl(0 0% 0% / 0.18), 2px 4px 6px 3px hsl(0 0% 0% / 0.18)",
            ShadowXl = "2px 2px 10px 4px hsl(0 0% 0% / 0.18), 2px 8px 10px 3px hsl(0 0% 0% / 0.18)",
            Shadow2xl = "2px 2px 10px 4px hsl(0 0% 0% / 0.45)"
        });

    private static ThemePreset CreateModernMinimalPreset() => new(
        "Modern Minimal",
        ["oklch(0.6231 0.1880 259.8145)", "oklch(0.3791 0.1378 265.5222)", "oklch(0.3715 0 0)", "oklch(0.2046 0 0)"],
        new ThemeState
        {
            Radius = "0.375rem",
            Background = "oklch(0.2046 0 0)",
            Foreground = "oklch(0.9219 0 0)",
            Card = "oklch(0.2686 0 0)",
            CardForeground = "oklch(0.9219 0 0)",
            Popover = "oklch(0.2686 0 0)",
            PopoverForeground = "oklch(0.9219 0 0)",
            Primary = "oklch(0.6231 0.1880 259.8145)",
            PrimaryForeground = "oklch(1.0000 0 0)",
            Secondary = "oklch(0.2686 0 0)",
            SecondaryForeground = "oklch(0.9219 0 0)",
            Muted = "oklch(0.2393 0 0)",
            MutedForeground = "oklch(0.7155 0 0)",
            Accent = "oklch(0.3791 0.1378 265.5222)",
            AccentForeground = "oklch(0.8823 0.0571 254.1284)",
            Destructive = "oklch(0.6368 0.2078 25.3313)",
            DestructiveForeground = "oklch(1.0000 0 0)",
            Border = "oklch(0.3715 0 0)",
            Input = "oklch(0.3715 0 0)",
            Ring = "oklch(0.6231 0.1880 259.8145)",
            Chart1 = "oklch(0.7137 0.1434 254.6240)",
            Chart2 = "oklch(0.6231 0.1880 259.8145)",
            Chart3 = "oklch(0.5461 0.2152 262.8809)",
            Chart4 = "oklch(0.4882 0.2172 264.3763)",
            Chart5 = "oklch(0.4244 0.1809 265.6377)",
            Sidebar = "oklch(0.2046 0 0)",
            SidebarForeground = "oklch(0.9219 0 0)",
            SidebarPrimary = "oklch(0.6231 0.1880 259.8145)",
            SidebarPrimaryForeground = "oklch(1.0000 0 0)",
            SidebarAccent = "oklch(0.3791 0.1378 265.5222)",
            SidebarAccentForeground = "oklch(0.8823 0.0571 254.1284)",
            SidebarBorder = "oklch(0.3715 0 0)",
            SidebarRing = "oklch(0.6231 0.1880 259.8145)",
            FontSans = "Inter, sans-serif",
            FontSerif = "Source Serif 4, serif",
            FontMono = "JetBrains Mono, monospace",
            TrackingNormal = "0em",
            Spacing = "0.25rem",
            ShadowColor = "oklch(0 0 0)",
            ShadowOpacity = "0.1",
            ShadowBlur = "3px",
            ShadowSpread = "0px",
            ShadowX = "0",
            ShadowY = "1px",
            Shadow2xs = "0 1px 3px 0px hsl(0 0% 0% / 0.05)",
            ShadowXs = "0 1px 3px 0px hsl(0 0% 0% / 0.05)",
            ShadowSm = "0 1px 3px 0px hsl(0 0% 0% / 0.10), 0 1px 2px -1px hsl(0 0% 0% / 0.10)",
            Shadow = "0 1px 3px 0px hsl(0 0% 0% / 0.10), 0 1px 2px -1px hsl(0 0% 0% / 0.10)",
            ShadowMd = "0 1px 3px 0px hsl(0 0% 0% / 0.10), 0 2px 4px -1px hsl(0 0% 0% / 0.10)",
            ShadowLg = "0 1px 3px 0px hsl(0 0% 0% / 0.10), 0 4px 6px -1px hsl(0 0% 0% / 0.10)",
            ShadowXl = "0 1px 3px 0px hsl(0 0% 0% / 0.10), 0 8px 10px -1px hsl(0 0% 0% / 0.10)",
            Shadow2xl = "0 1px 3px 0px hsl(0 0% 0% / 0.25)"
        });
}








