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
    public ThemeStateFull? CurrentTheme { get; private set; }

    /// <summary>
    /// Gets the current runtime stylesheet text injected by <c>App.razor</c>.
    /// </summary>
    public string RuntimeStyleSheet => CurrentTheme is null ? string.Empty : ThemeStateFullConverter.ToStyleSheet(CurrentTheme);

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

    private List<ThemePreset> _themes;
    private readonly ThemeFetcher _themeFetcher;
    private readonly ThemeInterop _themeInterop;
    private readonly HttpClient _httpClient;

    private readonly SemaphoreSlim _fontCatalogLock = new(1, 1);
    private readonly SemaphoreSlim _externalThemeLock = new(1, 1);

    // Maps a font stylesheet URL to the inflight (or completed) Task that injects it.
    // Concurrent callers await the same Task; failed Tasks are removed so they can be retried.
    private readonly Dictionary<string, Task> _fontLoadTasks = new(StringComparer.Ordinal);
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

        // CurrentTheme starts as null. The style block will yield nothing until
        // the baseline theme finishes fetching from tw_in.css.
        CurrentTheme = null;

        // Presets start empty — they are populated asynchronously by EnsureExternalThemesLoadedAsync.
        // The Customize page picker will be empty until that fetch completes, which is intentional.
        _themes = [];
    }

    public async Task EnsureExternalThemesLoadedAsync(CancellationToken cancellationToken = default)
    {
        // Fast-path: already loaded, no lock needed.
        if (_loadedExternalThemes)
            return;

        await _externalThemeLock.WaitAsync(cancellationToken);
        try
        {
            // Double-check inside the lock to handle concurrent callers.
            if (_loadedExternalThemes)
                return;

            // Console.WriteLine("Starting background load of external themes and base tw_in.css constraints.");

            var twInBaseTask = _httpClient.GetStringAsync("_content/ShadcnBlazor/css/tw_in.css", cancellationToken);
            
            var presetTasks = ThemeFetcher.BuiltInThemes
                .Select(themeName => _themeFetcher.TryGetPresetAsync(themeName, cancellationToken))
                .ToArray();
            
            var presets = await Task.WhenAll(presetTasks);

            // Harvest the base theme structure from the library CSS immediately.
            try
            {
                var baseCss = await twInBaseTask;
                // Console.WriteLine($"Fetched tw_in.css [{baseCss.Length} bytes]. Content being processed:\n{baseCss}");
                CurrentTheme = ThemeStateFullConverter.FromStyleSheet(baseCss);
                ThemeChanged?.Invoke(); // Tell the UI to immediately paint the base theme!
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Failed to load _content/ShadcnBlazor/css/tw_in.css during theme initialization. Base theme will be empty on startup. Exception: {ex.Message}");
            }

            var filtered = presets
                .Where(preset => preset is not null)
                .Cast<ThemePreset>()
                .ToList();

            // Atomically replace the backing list so no consumer ever sees a partially-mutated
            // collection during a Blazor render cycle (avoids "collection was modified" exceptions).
            // Do NOT overwrite CurrentTheme here — it is owned by the base CSS block above.
            _themes = filtered;
            _loadedExternalThemes = true;
            
            // Console.WriteLine("Completed loading external themes and base tw_in.css constraints.");
        }
        finally
        {
            _externalThemeLock.Release();
        }
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
        return EnsureStylesheetLoadedAsync(previewUrl, cancellationToken);
    }

    public Task EnsureFontFullLoadedAsync(FontMetadata font, CancellationToken cancellationToken = default)
    {
        var fullUrl = BuildGoogleFontsUrl(font.Family, preview: false);
        return EnsureStylesheetLoadedAsync(fullUrl, cancellationToken);
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

    private Task EnsureStylesheetLoadedAsync(string url, CancellationToken cancellationToken)
    {
        Task? existing;
        lock (_fontLoadSync)
        {
            if (_fontLoadTasks.TryGetValue(url, out existing))
            {
                // Return the already-inflight (or successfully completed) task so the caller
                // awaits the same work rather than returning immediately on a concurrent request.
                return existing;
            }

            // Register a placeholder *before* we start awaiting so that any concurrent caller
            // that arrives during the async gap will find it in the dictionary.
            var inject = InjectAndCleanupAsync(url, cancellationToken);
            _fontLoadTasks[url] = inject;
            return inject;
        }
    }

    private async Task InjectAndCleanupAsync(string url, CancellationToken cancellationToken)
    {
        try
        {
            await _themeInterop.InjectStylesheetAsync(url, cancellationToken);
        }
        catch
        {
            // Injection failed — remove the entry so future callers can retry rather than
            // silently receiving a completed-but-never-injected stylesheet.
            lock (_fontLoadSync)
            {
                _fontLoadTasks.Remove(url);
            }
            throw;
        }
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

}




