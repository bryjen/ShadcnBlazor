using System.Globalization;
using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Select;
using ShadcnBlazor.Docs.Models.Theme;
using ShadcnBlazor.Docs.Services.Theme;

namespace ShadcnBlazor.Docs.Pages.Customize.Tabs;

public partial class ValuesTab : ComponentBase, IDisposable
{
    private const string SansSerifCategory = "sans-serif";
    private const string SerifCategory = "serif";
    private const string MonospaceCategory = "monospace";

    private const string TrackingNormalVar = "--tracking-normal";
    private const string SpacingVar = "--spacing";
    private const string RadiusVar = "--radius";
    private const string ShadowBlurVar = "--shadow-blur";
    private const string ShadowSpreadVar = "--shadow-spread";
    private const string Shadow2xsVar = "--shadow-2xs";
    private const string ShadowXsVar = "--shadow-xs";
    private const string ShadowSmVar = "--shadow-sm";
    private const string ShadowVar = "--shadow";
    private const string ShadowMdVar = "--shadow-md";
    private const string ShadowLgVar = "--shadow-lg";
    private const string ShadowXlVar = "--shadow-xl";
    private const string Shadow2xlVar = "--shadow-2xl";

    [Inject]
    public required ThemeService ThemeService { get; set; }

    private bool _loadingPresets;
    private bool _loadingFonts;

    private ThemePreset? _selectedThemePreset;

    private List<SelectOption<FontMetadata>> _sansSerifFontOptions = [];
    private List<SelectOption<FontMetadata>> _serifFontOptions = [];
    private List<SelectOption<FontMetadata>> _monospaceFontOptions = [];

    private FontMetadata? _selectedSansSerifFont;
    private FontMetadata? _selectedSerifFont;
    private FontMetadata? _selectedMonospaceFont;

    private double _letterSpacingEm;
    private double _lineSpacingRem;
    private double _radiusRem;
    private double _shadowBlurPx;
    private double _shadowSpreadPx;
    private string LetterSpacingInputValue => FormatNumber(_letterSpacingEm);
    private string LineSpacingInputValue => FormatNumber(_lineSpacingRem);
    private string RadiusInputValue => FormatNumber(_radiusRem);
    private string ShadowBlurInputValue => FormatNumber(_shadowBlurPx);
    private string ShadowSpreadInputValue => FormatNumber(_shadowSpreadPx);

    protected override void OnInitialized()
    {
        // ValuesTabColors.cs
        BuildSectionsFromTheme();
        ThemeService.ThemeChanged += OnThemeChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        _loadingPresets = true;
        _loadingFonts = true;
        StateHasChanged();

        await Task.WhenAll(
            ThemeService.EnsureExternalThemesLoadedAsync(),
            ThemeService.EnsureFontCatalogLoadedAsync());

        BuildTypographyOptions();
        SyncSelectedFontsFromTheme();
        SyncNumericControlsFromTheme();

        await Task.WhenAll(
            EnsureSelectedFontsLoadedAsync(),
            PreloadInitialFontPreviewsAsync());

        _loadingPresets = false;
        _loadingFonts = false;
        StateHasChanged();
    }

    public void Dispose()
    {
        // ValuesTabColors.cs
        ThemeService.ThemeChanged -= OnThemeChanged;
    }

    private IReadOnlyList<SelectOption<ThemePreset>> GetThemeOptions() =>
        ThemeService.Presets.Select(preset => new SelectOption<ThemePreset>(preset, preset.Name)).ToList();

    private async Task OnThemePresetChanged(ThemePreset? option)
    {
        _selectedThemePreset = option;
        if (_selectedThemePreset is null)
        {
            return;
        }

        await ThemeService.ApplyPresetAsync(_selectedThemePreset);
    }

    private void OnThemeUpdated()
    {
        SyncSelectedFontsFromTheme();
        SyncNumericControlsFromTheme();
        _ = EnsureSelectedFontsLoadedAsync();
    }

    private void BuildTypographyOptions()
    {
        _sansSerifFontOptions = BuildFontOptions(SansSerifCategory);
        _serifFontOptions = BuildFontOptions(SerifCategory);
        _monospaceFontOptions = BuildFontOptions(MonospaceCategory);
    }

    private List<SelectOption<FontMetadata>> BuildFontOptions(string category)
    {
        var fonts = ThemeService.GetFontsByCategory(category)
            .OrderBy(font => font.Family, StringComparer.OrdinalIgnoreCase)
            .Select(font => new SelectOption<FontMetadata>(font, font.Family))
            .ToList();

        return fonts;
    }

    private void SyncSelectedFontsFromTheme()
    {
        _selectedSansSerifFont = FindSelectedFont(SansSerifCategory, ThemeService.CurrentTheme.Shared.FontSans);
        _selectedSerifFont = FindSelectedFont(SerifCategory, ThemeService.CurrentTheme.Shared.FontSerif);
        _selectedMonospaceFont = FindSelectedFont(MonospaceCategory, ThemeService.CurrentTheme.Shared.FontMono);
    }

    private FontMetadata? FindSelectedFont(string category, string themeToken)
    {
        var family = ExtractPrimaryFamily(themeToken);
        if (family is null)
        {
            return null;
        }

        return ThemeService.GetFontsByCategory(category)
            .FirstOrDefault(font => string.Equals(font.Family, family, StringComparison.OrdinalIgnoreCase));
    }

    private static string? ExtractPrimaryFamily(string? fontToken)
    {
        if (string.IsNullOrWhiteSpace(fontToken))
        {
            return null;
        }

        var trimmed = fontToken.Trim();
        if (trimmed.StartsWith('"'))
        {
            var end = trimmed.IndexOf('"', 1);
            if (end > 1)
            {
                return trimmed[1..end].Trim();
            }
        }

        var comma = trimmed.IndexOf(',');
        var first = comma >= 0 ? trimmed[..comma] : trimmed;
        first = first.Trim().Trim('"', '\'');

        return string.IsNullOrWhiteSpace(first)
            ? null
            : first;
    }

    private async Task OnSansSerifFontChanged(FontMetadata? font)
    {
        if (font is null)
        {
            return;
        }

        _selectedSansSerifFont = font;
        await ThemeService.SetFontAsync(SansSerifCategory, font);
    }

    private async Task OnSerifFontChanged(FontMetadata? font)
    {
        if (font is null)
        {
            return;
        }

        _selectedSerifFont = font;
        await ThemeService.SetFontAsync(SerifCategory, font);
    }

    private async Task OnMonospaceFontChanged(FontMetadata? font)
    {
        if (font is null)
        {
            return;
        }

        _selectedMonospaceFont = font;
        await ThemeService.SetFontAsync(MonospaceCategory, font);
    }

    private async Task EnsureSelectedFontsLoadedAsync()
    {
        var tasks = new List<Task>(3);
        if (_selectedSansSerifFont is not null)
        {
            tasks.Add(ThemeService.EnsureFontFullLoadedAsync(_selectedSansSerifFont));
        }

        if (_selectedSerifFont is not null)
        {
            tasks.Add(ThemeService.EnsureFontFullLoadedAsync(_selectedSerifFont));
        }

        if (_selectedMonospaceFont is not null)
        {
            tasks.Add(ThemeService.EnsureFontFullLoadedAsync(_selectedMonospaceFont));
        }

        if (tasks.Count > 0)
        {
            await Task.WhenAll(tasks);
        }
    }

    private async Task PreloadInitialFontPreviewsAsync()
    {
        var tasks = new List<Task>();

        tasks.AddRange(ThemeService.GetFontsByCategory(SansSerifCategory)
            .Take(12)
            .Select(font => ThemeService.EnsureFontPreviewLoadedAsync(font)));
        tasks.AddRange(ThemeService.GetFontsByCategory(SerifCategory)
            .Take(12)
            .Select(font => ThemeService.EnsureFontPreviewLoadedAsync(font)));
        tasks.AddRange(ThemeService.GetFontsByCategory(MonospaceCategory)
            .Take(12)
            .Select(font => ThemeService.EnsureFontPreviewLoadedAsync(font)));

        if (tasks.Count > 0)
        {
            await Task.WhenAll(tasks);
        }
    }

    private void SyncNumericControlsFromTheme()
    {
        var darkVars = ThemeService.CurrentTheme.Dark.ToCssVarMap();

        _letterSpacingEm = ParseUnitValue(darkVars, TrackingNormalVar, "em", 0);
        _lineSpacingRem = ParseUnitValue(darkVars, SpacingVar, "rem", 0.25);
        _radiusRem = ParseUnitValue(darkVars, RadiusVar, "rem", 0.65);
        _shadowBlurPx = ParseUnitValue(darkVars, ShadowBlurVar, "px", 3);
        _shadowSpreadPx = ParseUnitValue(darkVars, ShadowSpreadVar, "px", 0);
    }

    private async Task OnLetterSpacingSliderChanged(double value)
    {
        _letterSpacingEm = Clamp(value, -0.10, 0.25);
        await ThemeService.SetNonColorVarsAsync(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [TrackingNormalVar] = FormatWithUnit(_letterSpacingEm, "em")
        });
    }

    private async Task OnLineSpacingSliderChanged(double value)
    {
        _lineSpacingRem = Clamp(value, 0.125, 1.0);
        await ThemeService.SetNonColorVarsAsync(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [SpacingVar] = FormatWithUnit(_lineSpacingRem, "rem")
        });
    }

    private async Task OnRadiusSliderChanged(double value)
    {
        _radiusRem = Clamp(value, 0, 2.0);
        await ThemeService.SetNonColorVarsAsync(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [RadiusVar] = FormatWithUnit(_radiusRem, "rem")
        });
    }

    private async Task OnShadowBlurSliderChanged(double value)
    {
        _shadowBlurPx = Clamp(value, 0, 64);
        await PersistShadowVarsAsync();
    }

    private async Task OnShadowSpreadSliderChanged(double value)
    {
        _shadowSpreadPx = Clamp(value, -50, 50);
        await PersistShadowVarsAsync();
    }

    private async Task OnLetterSpacingInputChanged(ChangeEventArgs args)
    {
        if (!TryParseNumericInput(args.Value?.ToString(), out var value))
        {
            SyncNumericControlsFromTheme();
            return;
        }

        await OnLetterSpacingSliderChanged(value);
    }

    private async Task OnLineSpacingInputChanged(ChangeEventArgs args)
    {
        if (!TryParseNumericInput(args.Value?.ToString(), out var value))
        {
            SyncNumericControlsFromTheme();
            return;
        }

        await OnLineSpacingSliderChanged(value);
    }

    private async Task OnRadiusInputChanged(ChangeEventArgs args)
    {
        if (!TryParseNumericInput(args.Value?.ToString(), out var value))
        {
            SyncNumericControlsFromTheme();
            return;
        }

        await OnRadiusSliderChanged(value);
    }

    private async Task OnShadowBlurInputChanged(ChangeEventArgs args)
    {
        if (!TryParseNumericInput(args.Value?.ToString(), out var value))
        {
            SyncNumericControlsFromTheme();
            return;
        }

        await OnShadowBlurSliderChanged(value);
    }

    private async Task OnShadowSpreadInputChanged(ChangeEventArgs args)
    {
        if (!TryParseNumericInput(args.Value?.ToString(), out var value))
        {
            SyncNumericControlsFromTheme();
            return;
        }

        await OnShadowSpreadSliderChanged(value);
    }

    private async Task PersistShadowVarsAsync()
    {
        var dark = ThemeService.CurrentTheme.Dark;

        var shadowX = NormalizeLengthToken(dark.ShadowX, "0px");
        var shadowY = NormalizeLengthToken(dark.ShadowY, "1px");
        var shadowColor = string.IsNullOrWhiteSpace(dark.ShadowColor)
            ? "oklch(0 0 0)"
            : dark.ShadowColor.Trim();
        var shadowOpacity = Clamp(ParseRawNumber(dark.ShadowOpacity, 0.1), 0, 1);

        var blur = Math.Max(0, _shadowBlurPx);
        var spread = _shadowSpreadPx;

        var updates = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [ShadowBlurVar] = FormatWithUnit(blur, "px"),
            [ShadowSpreadVar] = FormatWithUnit(spread, "px"),
            [Shadow2xsVar] = BuildShadowLayer(shadowX, shadowY, blur * 0.5, spread, shadowColor, shadowOpacity * 0.5),
            [ShadowXsVar] = BuildShadowLayer(shadowX, shadowY, blur * 0.75, spread, shadowColor, shadowOpacity * 0.5),
            [ShadowSmVar] = BuildTwoLayerShadow(shadowX, shadowY, blur, spread, shadowColor, shadowOpacity, 0.66, -1, 0.8),
            [ShadowVar] = BuildTwoLayerShadow(shadowX, shadowY, blur, spread, shadowColor, shadowOpacity, 0.66, -1, 0.8),
            [ShadowMdVar] = BuildTwoLayerShadow(shadowX, shadowY, blur * 1.25, spread, shadowColor, shadowOpacity, 0.8, -1, 0.85),
            [ShadowLgVar] = BuildTwoLayerShadow(shadowX, shadowY, blur * 1.6, spread, shadowColor, shadowOpacity, 1.0, -1, 0.9),
            [ShadowXlVar] = BuildTwoLayerShadow(shadowX, shadowY, blur * 2.0, spread, shadowColor, shadowOpacity, 1.2, -1, 0.95),
            [Shadow2xlVar] = BuildShadowLayer(shadowX, shadowY, blur * 2.4, spread, shadowColor, Math.Min(1, shadowOpacity * 2.5))
        };

        await ThemeService.SetNonColorVarsAsync(updates);
    }

    private static string BuildTwoLayerShadow(
        string x,
        string y,
        double blur,
        double spread,
        string color,
        double primaryOpacity,
        double secondaryBlurScale,
        double secondarySpreadOffset,
        double secondaryOpacityScale)
    {
        var first = BuildShadowLayer(x, y, blur, spread, color, primaryOpacity);
        var second = BuildShadowLayer(
            x,
            y,
            blur * secondaryBlurScale,
            spread + secondarySpreadOffset,
            color,
            primaryOpacity * secondaryOpacityScale);

        return $"{first}, {second}";
    }

    private static string BuildShadowLayer(string x, string y, double blur, double spread, string color, double opacity)
    {
        var safeOpacity = Clamp(opacity, 0, 1);
        return $"{x} {y} {FormatWithUnit(Math.Max(0, blur), "px")} {FormatWithUnit(spread, "px")} {BuildColorWithOpacity(color, safeOpacity)}";
    }

    private static string BuildColorWithOpacity(string color, double opacity)
    {
        var percent = FormatNumber(opacity * 100);
        return $"color-mix(in oklab, {color} {percent}%, transparent)";
    }

    private static double ParseUnitValue(
        IReadOnlyDictionary<string, string> vars,
        string key,
        string expectedUnit,
        double fallback)
    {
        if (!vars.TryGetValue(key, out var raw) || string.IsNullOrWhiteSpace(raw))
        {
            return fallback;
        }

        var trimmed = raw.Trim();
        if (trimmed.EndsWith(expectedUnit, StringComparison.OrdinalIgnoreCase))
        {
            trimmed = trimmed[..^expectedUnit.Length].Trim();
        }

        return ParseRawNumber(trimmed, fallback);
    }

    private static double ParseRawNumber(string? raw, double fallback)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return fallback;
        }

        if (double.TryParse(raw.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var invariant))
        {
            return invariant;
        }

        if (double.TryParse(raw.Trim(), NumberStyles.Float, CultureInfo.CurrentCulture, out var current))
        {
            return current;
        }

        return fallback;
    }

    private static bool TryParseNumericInput(string? raw, out double value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }

        var trimmed = raw.Trim().ToLowerInvariant();
        foreach (var suffix in new[] { "rem", "em", "px" })
        {
            if (trimmed.EndsWith(suffix, StringComparison.Ordinal))
            {
                trimmed = trimmed[..^suffix.Length].Trim();
                break;
            }
        }

        return double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out value) ||
               double.TryParse(trimmed, NumberStyles.Float, CultureInfo.CurrentCulture, out value);
    }

    private static string NormalizeLengthToken(string? raw, string fallback)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return fallback;
        }

        var trimmed = raw.Trim();
        return trimmed;
    }

    private static string FormatNumber(double value)
        => value.ToString("0.###", CultureInfo.InvariantCulture);

    private static string FormatWithUnit(double value, string unit)
        => $"{FormatNumber(value)}{unit}";

    private static double Clamp(double value, double min, double max)
        => Math.Clamp(value, min, max);
}

