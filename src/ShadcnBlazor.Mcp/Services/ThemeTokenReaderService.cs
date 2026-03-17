using System.Reflection;
using System.Text.RegularExpressions;

namespace ShadcnBlazor.Mcp.Services;

public record ThemeToken(string Name, string Value, string Category);

public class ThemeTokenReaderService
{
    private readonly Lazy<IReadOnlyDictionary<string, List<ThemeToken>>> _tokens;

    public ThemeTokenReaderService()
    {
        _tokens = new Lazy<IReadOnlyDictionary<string, List<ThemeToken>>>(Parse);
    }

    public IReadOnlyDictionary<string, List<ThemeToken>> GetAll() => _tokens.Value;

    private static IReadOnlyDictionary<string, List<ThemeToken>> Parse()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("tw_in.css");
        if (stream is null) return new Dictionary<string, List<ThemeToken>>();

        using var reader = new StreamReader(stream);
        var css = reader.ReadToEnd();

        var result = new Dictionary<string, List<ThemeToken>>();
        var tokenRegex = new Regex(@"(--[\w-]+)\s*:\s*([^;]+);", RegexOptions.Multiline);

        foreach (Match match in tokenRegex.Matches(css))
        {
            var name = match.Groups[1].Value.Trim();
            var value = match.Groups[2].Value.Trim();
            var category = GetCategory(name);

            if (!result.ContainsKey(category))
                result[category] = [];

            // Avoid duplicates (dark mode re-declares same names)
            if (!result[category].Any(t => t.Name == name))
                result[category].Add(new ThemeToken(name, value, category));
        }

        return result;
    }

    private static string GetCategory(string name) => name switch
    {
        _ when name.StartsWith("--radius") => "radius",
        _ when name.StartsWith("--shadow") => "shadow",
        _ when name.StartsWith("--font-") || name.StartsWith("--tracking-") => "typography",
        _ when name.StartsWith("--spacing") => "spacing",
        _ when name is "--background-black" or "--background-dark" or "--card-dark" or "--border-dark"
            || name.StartsWith("--scrollbar-") || name.StartsWith("--chat-")
            || name is "--sidebar-text" or "--sidebar-header" => "extended",
        _ when name.StartsWith("--background") || name.StartsWith("--foreground")
            || name.StartsWith("--primary") || name.StartsWith("--secondary")
            || name.StartsWith("--muted") || name.StartsWith("--accent")
            || name.StartsWith("--destructive") || name.StartsWith("--border")
            || name.StartsWith("--input") || name.StartsWith("--ring")
            || name.StartsWith("--card") || name.StartsWith("--popover")
            || name.StartsWith("--chart-") || name.StartsWith("--sidebar-") => "color",
        _ => "other"
    };
}
