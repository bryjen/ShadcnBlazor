using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;

namespace ShadcnBlazor.Docs.Pages.Customize.Tabs;

public partial class ValuesTab : ComponentBase, IDisposable
{
    private List<ColorSection> Sections { get; set; } = [];

    private static readonly (string Title, (string Label, string CssVar)[] Fields)[] SectionDefinitions =
    [
        ("Primary Colors", [("Primary", "--primary"), ("Primary Foreground", "--primary-foreground")]),
        ("Secondary Colors", [("Secondary", "--secondary"), ("Secondary Foreground", "--secondary-foreground")]),
        ("Accent Colors", [("Accent", "--accent"), ("Accent Foreground", "--accent-foreground")]),
        ("Base Colors", [("Background", "--background"), ("Foreground", "--foreground")]),
        ("Card Colors", [("Card Background", "--card"), ("Card Foreground", "--card-foreground")]),
        ("Popover Colors", [("Popover Background", "--popover"), ("Popover Foreground", "--popover-foreground")]),
        ("Muted Colors", [("Muted", "--muted"), ("Muted Foreground", "--muted-foreground")]),
        ("Destructive Colors", [("Destructive", "--destructive"), ("Destructive Foreground", "--destructive-foreground")]),
        ("Border & Input Colors", [("Border", "--border"), ("Input", "--input"), ("Ring", "--ring")]),
        ("Chart Colors", [("Chart 1", "--chart-1"), ("Chart 2", "--chart-2"), ("Chart 3", "--chart-3"), ("Chart 4", "--chart-4"), ("Chart 5", "--chart-5")]),
        ("Sidebar Colors", [("Sidebar Background", "--sidebar"), ("Sidebar Foreground", "--sidebar-foreground"), ("Sidebar Primary", "--sidebar-primary"), ("Sidebar Primary Foreground", "--sidebar-primary-foreground"), ("Sidebar Accent", "--sidebar-accent"), ("Sidebar Accent Foreground", "--sidebar-accent-foreground"), ("Sidebar Border", "--sidebar-border"), ("Sidebar Ring", "--sidebar-ring")]),
    ];

    private static readonly Regex HexColorRegex = new(
        "^#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{4}|[0-9a-fA-F]{6}|[0-9a-fA-F]{8})$",
        RegexOptions.Compiled);

    private void OnThemeChanged()
    {
        BuildSectionsFromTheme();
        OnThemeUpdated();
        _ = InvokeAsync(StateHasChanged);
    }

    private void BuildSectionsFromTheme()
    {
        var darkVars = ThemeService.CurrentTheme.Dark.ToCssVarMap();

        Sections = SectionDefinitions
            .Select(section => new ColorSection(
                section.Title,
                section.Fields
                    .Select(field => new ColorField(
                        field.Label,
                        field.CssVar,
                        darkVars.TryGetValue(field.CssVar, out var value) ? value : string.Empty))
                    .ToList()))
            .ToList();
    }

    private async Task UpdateFieldValueAsync(string cssVarName, string value)
    {
        var field = Sections
            .SelectMany(section => section.Fields)
            .FirstOrDefault(current => string.Equals(current.CssVarName, cssVarName, StringComparison.Ordinal));

        if (field is null)
        {
            return;
        }

        field.Value = value;

        var next = ThemeService.CurrentTheme.Clone();
        next.Dark.ApplyCssVarMap(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [cssVarName] = value
        });

        await ThemeService.SaveThemeAsync(next);
    }

    private async Task UpdateColorPickerValueAsync(string cssVarName, string? value)
    {
        if (!TryNormalizeHexColor(value, out var normalized))
        {
            return;
        }

        await UpdateFieldValueAsync(cssVarName, normalized);
    }

    private static string GetColorPickerValue(string value)
        => TryNormalizeHexColor(value, out var normalized) ? normalized : "#000000";

    private static bool TryNormalizeHexColor(string? value, out string normalized)
    {
        normalized = "#000000";

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var trimmed = value.Trim();
        if (HexColorRegex.IsMatch(trimmed))
        {
            var hex = trimmed[1..];

            if (hex.Length is 3 or 4)
            {
                var r = hex[0];
                var g = hex[1];
                var b = hex[2];
                normalized = $"#{r}{r}{g}{g}{b}{b}";
                return true;
            }

            normalized = hex.Length == 8
                ? $"#{hex[..6]}"
                : $"#{hex}";

            return true;
        }

        return TryConvertOklchToHex(trimmed, out normalized);
    }

    private static bool TryConvertOklchToHex(string cssColor, out string hex)
    {
        hex = "#000000";

        if (!cssColor.StartsWith("oklch(", StringComparison.OrdinalIgnoreCase) ||
            !cssColor.EndsWith(')'))
        {
            return false;
        }

        var inner = cssColor[6..^1].Trim();
        if (inner.StartsWith("from ", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var coordPart = inner.Split('/', 2)[0].Trim();
        var parts = coordPart.Split([' ', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 3)
        {
            return false;
        }

        if (!TryParseCssNumber(parts[0], out var l, percentIsFraction: true) ||
            !TryParseCssNumber(parts[1], out var c, percentIsFraction: true) ||
            !TryParseHueDegrees(parts[2], out var h))
        {
            return false;
        }

        var lClamped = Clamp01(l);
        var cClamped = Math.Max(0, c);

        var hRad = (h % 360) * Math.PI / 180.0;
        var a = cClamped * Math.Cos(hRad);
        var b = cClamped * Math.Sin(hRad);

        var lPrime = lClamped + (0.3963377774 * a) + (0.2158037573 * b);
        var mPrime = lClamped - (0.1055613458 * a) - (0.0638541728 * b);
        var sPrime = lClamped - (0.0894841775 * a) - (1.2914855480 * b);

        var lCube = lPrime * lPrime * lPrime;
        var mCube = mPrime * mPrime * mPrime;
        var sCube = sPrime * sPrime * sPrime;

        var rLin = (4.0767416621 * lCube) - (3.3077115913 * mCube) + (0.2309699292 * sCube);
        var gLin = (-1.2684380046 * lCube) + (2.6097574011 * mCube) - (0.3413193965 * sCube);
        var bLin = (-0.0041960863 * lCube) - (0.7034186147 * mCube) + (1.7076147010 * sCube);

        var r = LinearToSrgbByte(rLin);
        var g = LinearToSrgbByte(gLin);
        var bl = LinearToSrgbByte(bLin);

        hex = $"#{r:X2}{g:X2}{bl:X2}";
        return true;
    }

    private static bool TryParseCssNumber(string token, out double value, bool percentIsFraction)
    {
        value = 0;
        var trimmed = token.Trim().TrimEnd(',');

        if (trimmed.EndsWith('%'))
        {
            if (!double.TryParse(trimmed[..^1], NumberStyles.Float, CultureInfo.InvariantCulture, out var percent))
            {
                return false;
            }

            value = percentIsFraction ? percent / 100.0 : percent;
            return true;
        }

        return double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    private static bool TryParseHueDegrees(string token, out double degrees)
    {
        degrees = 0;
        var trimmed = token.Trim().TrimEnd(',').ToLowerInvariant();

        if (trimmed.EndsWith("deg"))
        {
            return double.TryParse(trimmed[..^3], NumberStyles.Float, CultureInfo.InvariantCulture, out degrees);
        }

        if (trimmed.EndsWith("rad") &&
            double.TryParse(trimmed[..^3], NumberStyles.Float, CultureInfo.InvariantCulture, out var radians))
        {
            degrees = radians * (180.0 / Math.PI);
            return true;
        }

        if (trimmed.EndsWith("grad") &&
            double.TryParse(trimmed[..^4], NumberStyles.Float, CultureInfo.InvariantCulture, out var gradians))
        {
            degrees = gradians * 0.9;
            return true;
        }

        if (trimmed.EndsWith("turn") &&
            double.TryParse(trimmed[..^4], NumberStyles.Float, CultureInfo.InvariantCulture, out var turns))
        {
            degrees = turns * 360.0;
            return true;
        }

        return double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out degrees);
    }

    private static byte LinearToSrgbByte(double linear)
    {
        var clamped = Clamp01(linear);
        var srgb = clamped <= 0.0031308
            ? 12.92 * clamped
            : 1.055 * Math.Pow(clamped, 1.0 / 2.4) - 0.055;

        return (byte)Math.Round(Clamp01(srgb) * 255.0);
    }

    private static double Clamp01(double value)
        => value < 0 ? 0 : value > 1 ? 1 : value;

    private sealed class ColorSection(string title, List<ColorField> fields)
    {
        public string Title { get; } = title;
        public List<ColorField> Fields { get; } = fields;
    }

    private sealed class ColorField(string label, string cssVarName, string value)
    {
        public string Label { get; } = label;
        public string CssVarName { get; } = cssVarName;
        public string Value { get; set; } = value;
    }
}


