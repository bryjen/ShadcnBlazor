using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;

namespace ShadcnBlazor.Docs.Pages.Customize.Tabs;

public partial class ValuesTab : ComponentBase
{
    private readonly List<ColorSection> Sections =
    [
        new("Primary Colors", [ new("Primary", "#3b82f6"), new("Primary Foreground", "#ffffff") ]),
        new("Secondary Colors", [ new("Secondary", "#262626"), new("Secondary Foreground", "#e5e5e5") ]),
        new("Accent Colors", [ new("Accent", "#1e3a8a"), new("Accent Foreground", "#bfdbfe") ]),
        new("Base Colors", [ new("Background", "#171717"), new("Foreground", "#e5e5e5") ]),
        new("Card Colors", [ new("Card Background", "#262626"), new("Card Foreground", "#e5e5e5") ]),
        new("Popover Colors", [ new("Popover Background", "#262626"), new("Popover Foreground", "#e5e5e5") ]),
        new("Muted Colors", [ new("Muted", "#1f1f1f"), new("Muted Foreground", "#a3a3a3") ]),
        new("Destructive Colors", [ new("Destructive", "#ef4444"), new("Destructive Foreground", "#ffffff") ]),
        new("Border & Input Colors", [ new("Border", "#404040"), new("Input", "#404040"), new("Ring", "#3b82f6") ]),
        new("Chart Colors", [ new("Chart 1", "#60a5fa"), new("Chart 2", "#3b82f6"), new("Chart 3", "#2563eb"), new("Chart 4", "#1d4ed8"), new("Chart 5", "#1e40af") ]),
        new("Sidebar Colors", [ new("Sidebar Background", "#171717"), new("Sidebar Foreground", "#e5e5e5"), new("Sidebar Primary", "#3b82f6"), new("Sidebar Primary Foreground", "#ffffff"), new("Sidebar Accent", "#1e3a8a"), new("Sidebar Accent Foreground", "#bfdbfe"), new("Sidebar Border", "#404040"), new("Sidebar Ring", "#3b82f6") ]),
    ];

    private static readonly Regex CssColorRegex = new(
        @"^(#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{4}|[0-9a-fA-F]{6}|[0-9a-fA-F]{8})|(rgba?|hsla?|hwb|oklab|oklch|lab|lch|color)\([^\)]+\))$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static bool IsValidCssColor(string value)
        => !string.IsNullOrWhiteSpace(value) && CssColorRegex.IsMatch(value.Trim());

    private static string GetSwatchStyle(string value, bool isValid)
        => isValid
            ? $"background-color: {value.Trim()};"
            : "background: linear-gradient(135deg, transparent 42%, rgb(239 68 68 / 0.9) 42%, rgb(239 68 68 / 0.9) 58%, transparent 58%);";

    private void UpdateFieldValue(int sectionIndex, int fieldIndex, string value)
    {
        if (sectionIndex < 0 || sectionIndex >= Sections.Count)
        {
            return;
        }

        var fields = Sections[sectionIndex].Fields;
        if (fieldIndex < 0 || fieldIndex >= fields.Count)
        {
            return;
        }

        fields[fieldIndex].Value = value;
    }

    private sealed class ColorSection(string title, List<ColorField> fields)
    {
        public string Title { get; } = title;
        public List<ColorField> Fields { get; } = fields;
    }

    private sealed class ColorField(string label, string value)
    {
        public string Label { get; } = label;
        public string Value { get; set; } = value;
    }
}
