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

    private static readonly Regex CssColorRegex = new(
        @"^(#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{4}|[0-9a-fA-F]{6}|[0-9a-fA-F]{8})|(rgba?|hsla?|hwb|oklab|oklch|lab|lch|color)\([^\)]+\))$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    private void OnThemeChanged()
    {
        BuildSectionsFromTheme();
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

    private static bool IsValidCssColor(string value)
        => !string.IsNullOrWhiteSpace(value) && CssColorRegex.IsMatch(value.Trim());

    private static string GetSwatchStyle(string value, bool isValid)
        => isValid
            ? $"background-color: {value.Trim()};"
            : "background: linear-gradient(135deg, transparent 42%, rgb(239 68 68 / 0.9) 42%, rgb(239 68 68 / 0.9) 58%, transparent 58%);";

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