using System.Reflection;
using System.Text.Json;

namespace ShadcnBlazor.Mcp.Services;

public record ComponentExample(string Name, string Description, string RazorSnippet);

public class DocumentationReaderService
{
    private readonly Lazy<string> _conventions;
    private readonly Lazy<IReadOnlyDictionary<string, List<ComponentExample>>> _examples;

    public DocumentationReaderService()
    {
        _conventions = new Lazy<string>(ReadConventions);
        _examples = new Lazy<IReadOnlyDictionary<string, List<ComponentExample>>>(ReadExamples);
    }

    public string GetConventions() => _conventions.Value;

    public IReadOnlyList<ComponentExample> GetExamples(string componentName)
    {
        var key = _examples.Value.Keys.FirstOrDefault(k =>
            k.Equals(componentName, StringComparison.OrdinalIgnoreCase));
        return key is not null ? _examples.Value[key] : [];
    }

    private static string ReadConventions()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith("conventions.md"));
        if (resourceName is null) return string.Empty;

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static IReadOnlyDictionary<string, List<ComponentExample>> ReadExamples()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith("examples.json"));
        if (resourceName is null) return new Dictionary<string, List<ComponentExample>>();

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        var raw = JsonSerializer.Deserialize<Dictionary<string, List<ExampleDto>>>(stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return raw?.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Select(e => new ComponentExample(e.Name, e.Description, e.Razor)).ToList()
        ) ?? new Dictionary<string, List<ComponentExample>>();
    }

    private record ExampleDto(string Name, string Description, string Razor);
}
