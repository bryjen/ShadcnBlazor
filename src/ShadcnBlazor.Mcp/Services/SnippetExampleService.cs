using System.Reflection;
using ShadcnBlazor.Docs.Components.Docs.CodeBlock;
using ShadcnBlazor.Docs.Models;

namespace ShadcnBlazor.Mcp.Services;

public class SnippetExampleService
{
    private readonly Lazy<IReadOnlyList<SnippetEntry>> _entries = new(BuildEntries);

    public IReadOnlyList<ComponentExample> GetComponentExamples(string componentName)
    {
        if (string.IsNullOrWhiteSpace(componentName))
            return Array.Empty<ComponentExample>();

        return _entries.Value
            .Where(e => e.ComponentName.Equals(componentName, StringComparison.OrdinalIgnoreCase))
            .Select(e => new ComponentExample(e.Name, e.Description, e.RazorSnippet))
            .ToList();
    }

    private static IReadOnlyList<SnippetEntry> BuildEntries()
    {
        var result = new List<SnippetEntry>();

        foreach (var field in typeof(Snippets).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (field.FieldType != typeof(CodeFile))
                continue;

            if (field.GetValue(null) is not CodeFile codeFile)
                continue;

            var fieldName = field.Name;
            if (!TryParseComponentExample(fieldName, out var componentName, out var exampleName))
                continue;

            var name = GetNameFromFile(codeFile.FileName) ?? exampleName;
            var description = "Docs example";

            result.Add(new SnippetEntry(
                componentName,
                name,
                description,
                codeFile.Contents
            ));
        }

        return result;
    }

    private static bool TryParseComponentExample(string fieldName, out string componentName, out string exampleName)
    {
        componentName = string.Empty;
        exampleName = string.Empty;

        const string prefix = "Components_";
        const string examplesMarker = "_Examples_";

        if (!fieldName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return false;

        var markerIndex = fieldName.IndexOf(examplesMarker, StringComparison.OrdinalIgnoreCase);
        if (markerIndex < 0)
            return false;

        componentName = fieldName.Substring(prefix.Length, markerIndex - prefix.Length);
        exampleName = fieldName[(markerIndex + examplesMarker.Length)..];
        return !string.IsNullOrWhiteSpace(componentName) && !string.IsNullOrWhiteSpace(exampleName);
    }

    private static string? GetNameFromFile(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return null;

        var name = Path.GetFileNameWithoutExtension(fileName);
        return string.IsNullOrWhiteSpace(name) ? null : name;
    }

    private sealed record SnippetEntry(string ComponentName, string Name, string Description, string RazorSnippet);
}
