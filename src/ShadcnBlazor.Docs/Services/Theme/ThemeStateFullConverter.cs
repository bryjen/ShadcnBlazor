using System.Text.RegularExpressions;
using ShadcnBlazor.Docs.Models.Theme;

namespace ShadcnBlazor.Docs.Services.Theme;

/// <summary>
/// Converts full theme state to and from runtime CSS variable maps.
/// </summary>
public static class ThemeStateFullConverter
{
    private static readonly Regex ScopeBlockRegex = new(
        """(?<scope>:root|\.dark)\s*\{(?<body>[^}]*)\}""",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

    private static readonly Regex VarDeclarationRegex = new(
        """(?<name>--[a-z0-9-]+)\s*:\s*(?<value>[^;]+);""",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Builds the runtime variable map currently applied by the docs app (shared + dark).
    /// </summary>
    public static IReadOnlyDictionary<string, string> ToRuntimeCssVarMap(ThemeStateFull state)
    {
        ArgumentNullException.ThrowIfNull(state);

        var values = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var pair in state.Shared.ToCssVarMap())
        {
            values[pair.Key] = pair.Value;
        }

        foreach (var pair in state.Dark.ToCssVarMap())
        {
            values[pair.Key] = pair.Value;
        }

        return values;
    }

    /// <summary>
    /// Creates or updates a full theme state from a runtime map.
    /// Missing variables are preserved from <paramref name="baseline"/> when provided.
    /// </summary>
    public static ThemeStateFull FromRuntimeCssVarMap(
        IReadOnlyDictionary<string, string> values,
        ThemeStateFull? baseline = null)
    {
        ArgumentNullException.ThrowIfNull(values);

        var state = baseline?.Clone() ?? new ThemeStateFull();

        // Runtime map does not currently differentiate light/shared scopes,
        // so import into dark + shared while keeping light as-is.
        state.Dark.ApplyCssVarMap(values);
        state.Shared.ApplyCssVarMap(values);

        return state;
    }

    /// <summary>
    /// Parses stylesheet text into a full theme state, starting from defaults.
    /// </summary>
    public static ThemeStateFull FromStyleSheet(string css)
        => FromStyleSheet(css, baseline: null);

    /// <summary>
    /// Parses stylesheet text into a full theme state, overlaying values on an optional baseline.
    /// </summary>
    public static ThemeStateFull FromStyleSheet(string css, ThemeStateFull? baseline)
    {
        ArgumentNullException.ThrowIfNull(css);

        var state = baseline?.Clone() ?? new ThemeStateFull();
        var rootVars = new Dictionary<string, string>(StringComparer.Ordinal);
        var darkVars = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (Match scopeMatch in ScopeBlockRegex.Matches(css))
        {
            if (!scopeMatch.Success)
            {
                continue;
            }

            var scope = scopeMatch.Groups["scope"].Value;
            var body = scopeMatch.Groups["body"].Value;
            var target = string.Equals(scope, ".dark", StringComparison.OrdinalIgnoreCase)
                ? darkVars
                : rootVars;

            foreach (Match varMatch in VarDeclarationRegex.Matches(body))
            {
                if (!varMatch.Success)
                {
                    continue;
                }

                var name = varMatch.Groups["name"].Value.Trim();
                var value = varMatch.Groups["value"].Value.Trim();
                if (name.Length == 0 || value.Length == 0)
                {
                    continue;
                }

                target[name] = value;
            }
        }

        if (rootVars.Count > 0)
        {
            state.Light.ApplyCssVarMap(rootVars);
            state.Shared.ApplyCssVarMap(rootVars);
        }

        if (darkVars.Count > 0)
        {
            state.Dark.ApplyCssVarMap(darkVars);
        }

        return state;
    }

    /// <summary>
    /// Serializes full theme state to CSS for a runtime style block.
    /// </summary>
    public static string ToStyleSheet(ThemeStateFull state)
    {
        ArgumentNullException.ThrowIfNull(state);

        var rootVars = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var pair in state.Shared.ToCssVarMap())
        {
            rootVars[pair.Key] = pair.Value;
        }

        foreach (var pair in state.Light.ToCssVarMap())
        {
            rootVars[pair.Key] = pair.Value;
        }

        return string.Concat(
            SerializeSelector(":root", rootVars),
            "\n",
            SerializeSelector(".dark", state.Dark.ToCssVarMap()));
    }

    private static string SerializeSelector(string selector, IReadOnlyDictionary<string, string> values)
    {
        var lines = values
            .Where(pair => !string.IsNullOrWhiteSpace(pair.Value))
            .Select(pair => $"  {pair.Key}: {pair.Value};");

        return string.Join("\n", [$"{selector} {{", .. lines, "}"]);
    }
}
