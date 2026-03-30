using System.Text.Json;
using NUnit.Framework;
using ShadcnBlazor.Mcp.Services;
using ShadcnBlazor.Mcp.Tools;

namespace ShadcnBlazor.Mcp.Tests.Integration;

[TestFixture]
public class McpMetadataIntegrationTests
{
    private ComponentRegistryService _registry = null!;
    private DocumentationReaderService _docs = null!;
    private ApiDocumentationService _apiDocs = null!;
    private SnippetExampleService _snippets = null!;

    private ComponentTools _componentTools = null!;
    private DocumentationTools _docTools = null!;

    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = false };

    [SetUp]
    public void SetUp()
    {
        _registry = new ComponentRegistryService();
        _docs = new DocumentationReaderService();
        _apiDocs = new ApiDocumentationService();
        _snippets = new SnippetExampleService();

        _componentTools = new ComponentTools(_registry, _docs, _apiDocs, _snippets);
        _docTools = new DocumentationTools(_docs, _snippets);
    }

    private static JsonElement ToJson(object result) =>
        JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(result, _jsonOptions));

    [Test]
    public void GetComponent_IncludesDocumentationSummary()
    {
        var result = ToJson(_componentTools.GetComponent("Accordion"));
        var documentation = result.GetProperty("documentation");
        var summary = documentation.GetProperty("summary").GetString() ?? string.Empty;

        Console.WriteLine($"Summary length: {summary.Length}");
        Console.WriteLine($"Summary: {summary}");

        Assert.That(summary, Is.Not.Empty);
    }

    [Test]
    public void GetComponent_ParametersIncludeDefaultValueAndFlags()
    {
        var result = ToJson(_componentTools.GetComponent("Accordion"));
        var parameters = result.GetProperty("parameters").EnumerateArray().ToList();

        Assert.That(parameters, Is.Not.Empty);

        var sample = parameters.First();
        Assert.That(sample.TryGetProperty("defaultValue", out _), Is.True);
        Assert.That(sample.TryGetProperty("isRequired", out _), Is.True);
        Assert.That(sample.TryGetProperty("isCascading", out _), Is.True);
        Assert.That(sample.TryGetProperty("isEventCallback", out _), Is.True);

        Console.WriteLine($"Parameter fields: {string.Join(", ", sample.EnumerateObject().Select(p => p.Name))}");
    }

    [Test]
    public void GetExamples_UsesDocsSnippets_ForKnownSnippetComponent()
    {
        var snippetComponents = GetSnippetComponentNames().ToList();
        Assert.That(snippetComponents, Is.Not.Empty, "No snippet components found.");

        var componentName = snippetComponents[0];
        Console.WriteLine($"Using snippet component: {componentName}");

        var result = ToJson(_docTools.GetExamples(componentName));
        if (!result.TryGetProperty("examples", out var examplesProp))
        {
            var raw = JsonSerializer.Serialize(result, _jsonOptions);
            Assert.Fail($"Expected 'examples' property. Actual payload: {raw}");
        }

        var examples = examplesProp.EnumerateArray().ToList();

        Console.WriteLine($"Examples for {componentName}: {examples.Count}");
        if (examples.Count > 0)
        {
            var first = examples[0];
            var razor = GetStringProperty(first, "razorSnippet", "RazorSnippet");
            Console.WriteLine("First snippet (full):");
            WriteInChunks(razor);
        }

        var payload = JsonSerializer.Serialize(result, _jsonOptions);
        Console.WriteLine("Full payload:");
        WriteInChunks(payload);

        Assert.That(examples.Count, Is.GreaterThan(0));
    }

    [Test]
    public void GetComponent_WithoutApiDocs_StillReturnsParameters()
    {
        var result = ToJson(_componentTools.GetComponent("Drawer"));
        var documentation = result.GetProperty("documentation");
        var summary = documentation.GetProperty("summary").GetString() ?? string.Empty;
        var parameters = result.GetProperty("parameters");

        Console.WriteLine($"Drawer summary length: {summary.Length}");
        Console.WriteLine($"Parameters kind: {parameters.ValueKind}");

        Assert.That(parameters.ValueKind, Is.EqualTo(JsonValueKind.Array));
    }

    [Test]
    public void GetComponent_DumpsFullOutput()
    {
        var result = ToJson(_componentTools.GetComponent("Button"));
        var json = JsonSerializer.Serialize(result, _jsonOptions);

        Console.WriteLine("Full component payload:");
        WriteInChunks(json);

        Assert.That(result.ValueKind, Is.EqualTo(JsonValueKind.Object));
    }

    private static IEnumerable<string> GetSnippetComponentNames()
    {
        const string prefix = "Components_";
        const string marker = "_Examples_";

        foreach (var field in typeof(ShadcnBlazor.Docs.Models.Snippets).GetFields())
        {
            if (field.FieldType != typeof(ShadcnBlazor.Docs.Components.Docs.CodeBlock.CodeFile))
                continue;

            var name = field.Name;
            if (!name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                continue;

            var markerIndex = name.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (markerIndex < 0)
                continue;

            var component = name.Substring(prefix.Length, markerIndex - prefix.Length);
            if (!string.IsNullOrWhiteSpace(component))
                yield return component;
        }
    }

    private static string GetStringProperty(JsonElement element, params string[] names)
    {
        foreach (var name in names)
        {
            if (element.TryGetProperty(name, out var value))
                return value.GetString() ?? string.Empty;
        }

        return string.Empty;
    }

    private static void WriteInChunks(string text, int chunkSize = 800)
    {
        if (string.IsNullOrEmpty(text))
        {
            Console.WriteLine(string.Empty);
            return;
        }

        for (var i = 0; i < text.Length; i += chunkSize)
        {
            var len = Math.Min(chunkSize, text.Length - i);
            Console.WriteLine(text.Substring(i, len));
        }
    }
}
