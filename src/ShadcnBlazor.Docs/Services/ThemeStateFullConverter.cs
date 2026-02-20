namespace ShadcnBlazor.Docs.Services;

/// <summary>
/// Converts full theme state to and from runtime CSS variable maps.
/// </summary>
public static class ThemeStateFullConverter
{
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
