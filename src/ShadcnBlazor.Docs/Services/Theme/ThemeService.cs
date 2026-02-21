using System.Text.Json;
using ShadcnBlazor.Docs.Models.Theme;
using ShadcnBlazor.Docs.Services.Interop;

namespace ShadcnBlazor.Docs.Services.Theme;

/// <summary>
/// Theme service that manages the current theme state and exposes runtime stylesheet output.
/// </summary>
public sealed class ThemeService
{
    private const string SansSerifCategory = "sans-serif";
    private const string SerifCategory = "serif";
    private const string MonospaceCategory = "monospace";
    private const string FontCatalogFallbackPath = "data/font-catalog-fallback.json";

    private static readonly string[] FontCategories =
    [
        SansSerifCategory,
        SerifCategory,
        MonospaceCategory
    ];

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
    private readonly ThemeFetcher _themeFetcher;
    private readonly ThemeInterop _themeInterop;
    private readonly HttpClient _httpClient;

    private readonly SemaphoreSlim _fontCatalogLock = new(1, 1);
    private readonly HashSet<string> _loadedPreviewUrls = new(StringComparer.Ordinal);
    private readonly HashSet<string> _loadedFullUrls = new(StringComparer.Ordinal);
    private readonly object _fontLoadSync = new();

    private Dictionary<string, List<FontMetadata>> _fontsByCategory = new(StringComparer.Ordinal)
    {
        [SansSerifCategory] = [],
        [SerifCategory] = [],
        [MonospaceCategory] = []
    };

    private bool _loadedExternalThemes;
    private bool _loadedFontCatalog;

    /// <summary>
    /// Creates a new <see cref="ThemeService"/> instance.
    /// </summary>
    public ThemeService(ThemeFetcher themeFetcher, ThemeInterop themeInterop, HttpClient httpClient)
    {
        _themeFetcher = themeFetcher;
        _themeInterop = themeInterop;
        _httpClient = httpClient;

        CurrentTheme = CreateDefaultTheme();
        var themePreset = new ThemePreset("Default", ThemeFetcher.BuildSwatches(CurrentTheme), CurrentTheme.Clone());
        _themes = [themePreset];
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

    public async Task EnsureFontCatalogLoadedAsync(CancellationToken cancellationToken = default)
    {
        if (_loadedFontCatalog)
        {
            return;
        }

        await _fontCatalogLock.WaitAsync(cancellationToken);
        try
        {
            if (_loadedFontCatalog)
            {
                return;
            }

            var fetched = await TryLoadFontCatalogFromFontsourceAsync(cancellationToken);
            if (fetched is null)
            {
                fetched = await TryLoadFontCatalogFallbackAsync(cancellationToken);
            }

            if (fetched is not null)
            {
                _fontsByCategory = fetched;
                _loadedFontCatalog = true;
            }
        }
        finally
        {
            _fontCatalogLock.Release();
        }
    }

    public IReadOnlyList<FontMetadata> GetFontsByCategory(string category)
    {
        if (_fontsByCategory.TryGetValue(category, out var fonts))
        {
            return fonts;
        }

        return [];
    }

    public Task EnsureFontPreviewLoadedAsync(FontMetadata font, CancellationToken cancellationToken = default)
    {
        var previewUrl = BuildGoogleFontsUrl(font.Family, preview: true);
        return EnsureStylesheetLoadedAsync(previewUrl, _loadedPreviewUrls, cancellationToken);
    }

    public Task EnsureFontFullLoadedAsync(FontMetadata font, CancellationToken cancellationToken = default)
    {
        var fullUrl = BuildGoogleFontsUrl(font.Family, preview: false);
        return EnsureStylesheetLoadedAsync(fullUrl, _loadedFullUrls, cancellationToken);
    }

    public async Task SetFontAsync(string category, FontMetadata font, CancellationToken cancellationToken = default)
    {
        if (!TryGetFontCssVar(category, out var cssVarName))
        {
            return;
        }

        await EnsureFontFullLoadedAsync(font, cancellationToken);

        var next = CurrentTheme.Clone();
        next.Shared.ApplyCssVarMap(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [cssVarName] = BuildFontToken(font.Family, category)
        });

        await SaveThemeAsync(next, cancellationToken);
    }

    public async Task SetNonColorVarsAsync(
        IReadOnlyDictionary<string, string> values,
        CancellationToken cancellationToken = default)
    {
        if (values.Count == 0)
        {
            return;
        }

        var filtered = values
            .Where(pair => !string.IsNullOrWhiteSpace(pair.Key) && !string.IsNullOrWhiteSpace(pair.Value))
            .ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal);

        if (filtered.Count == 0)
        {
            return;
        }

        var next = CurrentTheme.Clone();
        next.Dark.ApplyCssVarMap(filtered);
        next.Shared.ApplyCssVarMap(filtered);

        await SaveThemeAsync(next, cancellationToken);
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

    private async Task EnsureStylesheetLoadedAsync(string url, HashSet<string> dedupeSet, CancellationToken cancellationToken)
    {
        var shouldLoad = false;
        lock (_fontLoadSync)
        {
            if (!dedupeSet.Contains(url))
            {
                dedupeSet.Add(url);
                shouldLoad = true;
            }
        }

        if (!shouldLoad)
        {
            return;
        }

        await _themeInterop.InjectStylesheetAsync(url, cancellationToken);
    }

    private async Task<Dictionary<string, List<FontMetadata>>?> TryLoadFontCatalogFromFontsourceAsync(CancellationToken cancellationToken)
    {
        try
        {
            var tasks = FontCategories
                .Select(category => FetchCategoryAsync(category, cancellationToken))
                .ToArray();

            var results = await Task.WhenAll(tasks);

            var catalog = new Dictionary<string, List<FontMetadata>>(StringComparer.Ordinal);
            for (var i = 0; i < FontCategories.Length; i++)
            {
                catalog[FontCategories[i]] = results[i];
            }

            return catalog.Values.All(list => list.Count > 0)
                ? catalog
                : null;
        }
        catch
        {
            return null;
        }
    }

    private async Task<List<FontMetadata>> FetchCategoryAsync(string category, CancellationToken cancellationToken)
    {
        var uri = new Uri($"https://api.fontsource.org/v1/fonts?category={Uri.EscapeDataString(category)}");
        using var response = await _httpClient.GetAsync(uri, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        if (document.RootElement.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        var fonts = new List<FontMetadata>();
        foreach (var item in document.RootElement.EnumerateArray())
        {
            var id = item.TryGetProperty("id", out var idEl)
                ? idEl.GetString()
                : null;
            var family = item.TryGetProperty("family", out var familyEl)
                ? familyEl.GetString()
                : null;
            var itemCategory = item.TryGetProperty("category", out var categoryEl)
                ? categoryEl.GetString()
                : category;

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(family) || string.IsNullOrWhiteSpace(itemCategory))
            {
                continue;
            }

            fonts.Add(new FontMetadata(id.Trim(), family.Trim(), itemCategory.Trim()));
        }

        return fonts;
    }

    private async Task<Dictionary<string, List<FontMetadata>>?> TryLoadFontCatalogFallbackAsync(CancellationToken cancellationToken)
    {
        try
        {
            var json = await _httpClient.GetStringAsync(FontCatalogFallbackPath, cancellationToken);
            using var document = JsonDocument.Parse(json);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            var catalog = new Dictionary<string, List<FontMetadata>>(StringComparer.Ordinal);
            foreach (var category in FontCategories)
            {
                var list = new List<FontMetadata>();
                if (document.RootElement.TryGetProperty(category, out var categoryElement) &&
                    categoryElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in categoryElement.EnumerateArray())
                    {
                        var id = item.TryGetProperty("id", out var idEl)
                            ? idEl.GetString()
                            : null;
                        var family = item.TryGetProperty("family", out var familyEl)
                            ? familyEl.GetString()
                            : null;
                        var fallbackCategory = item.TryGetProperty("category", out var categoryEl)
                            ? categoryEl.GetString()
                            : category;

                        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(family) || string.IsNullOrWhiteSpace(fallbackCategory))
                        {
                            continue;
                        }

                        list.Add(new FontMetadata(id.Trim(), family.Trim(), fallbackCategory.Trim()));
                    }
                }

                catalog[category] = list;
            }

            return catalog.Values.Any(list => list.Count > 0)
                ? catalog
                : null;
        }
        catch
        {
            return null;
        }
    }

    private static string BuildGoogleFontsUrl(string family, bool preview)
    {
        var encodedFamily = Uri.EscapeDataString(family).Replace("%20", "+", StringComparison.Ordinal);
        var baseUrl = $"https://fonts.googleapis.com/css2?family={encodedFamily}";

        if (preview)
        {
            var encodedText = Uri.EscapeDataString(family);
            return $"{baseUrl}&text={encodedText}&display=swap";
        }

        return $"{baseUrl}&display=swap";
    }

    private static bool TryGetFontCssVar(string category, out string cssVarName)
    {
        cssVarName = category switch
        {
            SansSerifCategory => "--font-sans",
            SerifCategory => "--font-serif",
            MonospaceCategory => "--font-mono",
            _ => string.Empty
        };

        return cssVarName.Length > 0;
    }

    private static string BuildFontToken(string family, string category)
    {
        var escapedFamily = family.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\"", "\\\"", StringComparison.Ordinal);
        var fallback = category switch
        {
            SansSerifCategory => "sans-serif",
            SerifCategory => "serif",
            MonospaceCategory => "monospace",
            _ => "sans-serif"
        };

        return $"\"{escapedFamily}\", {fallback}";
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




