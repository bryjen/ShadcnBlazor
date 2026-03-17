using System.Reflection;
using System.Text.Json;

namespace ShadcnBlazor.Mcp.Services;

public record KeyboardInteraction(string Key, string Action);

public record AccessibilitySpec(
    string ComponentName,
    string AriaRole,
    string[] RequiredAttributes,
    string[] OptionalAttributes,
    KeyboardInteraction[] KeyboardMap
);

public class AccessibilitySpecReaderService
{
    private readonly Lazy<IReadOnlyDictionary<string, AccessibilitySpec>> _specs;

    public AccessibilitySpecReaderService()
    {
        _specs = new Lazy<IReadOnlyDictionary<string, AccessibilitySpec>>(ReadSpecs);
    }

    public AccessibilitySpec? GetByComponent(string name)
    {
        var key = _specs.Value.Keys.FirstOrDefault(k =>
            k.Equals(name, StringComparison.OrdinalIgnoreCase));
        return key is not null ? _specs.Value[key] : null;
    }

    private static IReadOnlyDictionary<string, AccessibilitySpec> ReadSpecs()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith("accessibility.json"));
        if (resourceName is null) return new Dictionary<string, AccessibilitySpec>();

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        var list = JsonSerializer.Deserialize<List<AccessibilitySpec>>(stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return list?.ToDictionary(s => s.ComponentName, s => s)
               ?? new Dictionary<string, AccessibilitySpec>();
    }
}
