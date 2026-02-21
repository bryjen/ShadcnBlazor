using ShadcnBlazor.Docs.Models.Theme;

namespace ShadcnBlazor.Docs.Services.Theme;

public class ThemeFetcher(HttpClient http)
{
    public async Task<ThemePreset?> TryGetPresetAsync(
        string themeName,
        CancellationToken cancellationToken = default)
    {
        var uri = new Uri($"{http.BaseAddress?.AbsoluteUri}themes/{themeName}.css");
        var themeStateFull = await TryParseThemeAsync(uri, cancellationToken);
        if (themeStateFull is null) return null;

        var presetName = FormatThemeName(themeName);
        return new ThemePreset(presetName, BuildSwatches(themeStateFull), themeStateFull);
    }
    
    private async Task<ThemeStateFull?> TryParseThemeAsync(
        Uri uri,
        CancellationToken cancellationToken = default)
    {
        var css = await TryGetContentsAsync(uri, cancellationToken);
        if (css is null) return null;
        return ThemeStateFullConverter.FromStyleSheet(css);
    }

    private async Task<string?> TryGetContentsAsync(
        Uri uri,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await http.GetAsync(uri, cancellationToken);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
        catch
        {
            return null;
        }
    }
    
    public static string[] BuildSwatches(ThemeStateFull theme) =>
    [
        theme.Dark.Primary,
        theme.Dark.Secondary,
        theme.Dark.Accent,
        theme.Dark.Background
    ];
    
    public static readonly string[] BuiltInThemes = 
    [
        "brownie", 
        "claude", 
        "claude+", 
        "default", 
        "modern-minimal", 
        "portfolio-theme", 
        "sage-green", 
        "vtron", 
        "zen-inspired-theme",
        "verscrow-1.2"
    ];
    
    private static string FormatThemeName(string fileName) =>
        string.Join(' ', fileName
            .Replace('-', ' ')
            .Split(' ')
            .Select(word => char.ToUpper(word[0]) + word[1..]));
    
}