using ShadcnBlazor.Docs.Models;

namespace ShadcnBlazor.Mcp.Services;

public class ApiDocumentationService
{
    private readonly IReadOnlyDictionary<string, DocumentedType> _byKey;
    private readonly IReadOnlyDictionary<string, DocumentedType> _byName;

    public ApiDocumentationService()
    {
        _byKey = ApiDocumentation.All;
        _byName = _byKey.Values
            .GroupBy(d => Normalize(d.Name), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
    }

    public DocumentedType? Get(string componentName)
    {
        if (string.IsNullOrWhiteSpace(componentName))
            return null;

        if (_byKey.TryGetValue(componentName, out var exact))
            return exact;

        var normalized = Normalize(componentName);
        return _byName.TryGetValue(normalized, out var doc) ? doc : null;
    }

    private static string Normalize(string name)
    {
        var noTick = name.Split('`')[0];
        var noGeneric = noTick.Split('<')[0];
        return noGeneric.Trim();
    }
}
