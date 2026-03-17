using System.Text.Json;
using NUnit.Framework;
using ShadcnBlazor.Mcp.Services;
using ShadcnBlazor.Mcp.Tools;

namespace ShadcnBlazor.Mcp.Tests.Integration;

/// <summary>
/// Integration tests that exercise tool methods directly via instantiated services.
/// No HTTP/stdio transport is required.
/// </summary>
[TestFixture]
public class McpServerIntegrationTests
{
    private ComponentRegistryService _registry = null!;
    private ThemeTokenReaderService _theme = null!;
    private DocumentationReaderService _docs = null!;
    private AccessibilitySpecReaderService _a11y = null!;

    private ComponentTools _componentTools = null!;
    private ThemeTools _themeTools = null!;
    private DocumentationTools _docTools = null!;
    private AccessibilityTools _a11yTools = null!;

    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = false };

    [SetUp]
    public void SetUp()
    {
        _registry = new ComponentRegistryService();
        _theme = new ThemeTokenReaderService();
        _docs = new DocumentationReaderService();
        _a11y = new AccessibilitySpecReaderService();

        _componentTools = new ComponentTools(_registry);
        _themeTools = new ThemeTools(_theme);
        _docTools = new DocumentationTools(_docs);
        _a11yTools = new AccessibilityTools(_a11y);
    }

    private static JsonElement ToJson(object result) =>
        JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(result, _jsonOptions));

    [Test]
    public void ListComponents_ReturnsAllComponents()
    {
        var result = _componentTools.ListComponents();
        var list = result as System.Collections.IEnumerable;
        var items = list!.Cast<object>().ToList();
        var names = items
            .Select(o => ToJson(o).GetProperty("name").GetString())
            .Order()
            .ToList();

        Console.WriteLine($"Total components : {items.Count} (expected: >= 20)");
        Console.WriteLine($"Names: {string.Join(", ", names)}");

        Assert.That(list, Is.Not.Null);
        Assert.That(items.Count, Is.GreaterThanOrEqualTo(20));
    }

    [Test]
    public void GetComponent_ValidName_ReturnsParameters()
    {
        var result = ToJson(_componentTools.GetComponent("Button"));
        var parameters = result.GetProperty("parameters");
        var paramNames = parameters.EnumerateArray()
            .Select(p => p.GetProperty("name").GetString())
            .ToList();

        Console.WriteLine($"Component   : {result.GetProperty("name").GetString()}");
        Console.WriteLine($"Description : {result.GetProperty("description").GetString()}");
        Console.WriteLine($"Parameters  ({paramNames.Count}): {string.Join(", ", paramNames)}");
        Console.WriteLine($"Deps        : {string.Join(", ", result.GetProperty("dependencies").EnumerateArray().Select(d => d.GetString()))}");

        Assert.That(result.GetProperty("name").GetString(), Is.EqualTo("Button"));
        Assert.That(parameters.ValueKind, Is.EqualTo(JsonValueKind.Array));
    }

    [Test]
    public void GetComponent_InvalidName_ReturnsGracefulMessage()
    {
        var result = ToJson(_componentTools.GetComponent("Nonexistent"));
        var hasError = result.TryGetProperty("error", out var errorProp);

        Console.WriteLine($"Query       : \"Nonexistent\"");
        Console.WriteLine($"Has error   : {hasError} (expected: true)");
        Console.WriteLine($"Error msg   : {(hasError ? errorProp.GetString() : "(no error property)")}");

        Assert.DoesNotThrow(() => Assert.That(hasError, Is.True));
    }

    [Test]
    public void SearchComponents_ReturnsRelevantResults()
    {
        var result = ToJson(_componentTools.SearchComponents("select"));
        var names = result.GetProperty("results").EnumerateArray()
            .Select(e => e.GetProperty("name").GetString())
            .ToList();

        Console.WriteLine($"Query   : \"select\"");
        Console.WriteLine($"Matches : {string.Join(", ", names)}");
        Console.WriteLine($"Contains \"Select\": {names.Contains("Select")} (expected: true)");

        Assert.That(names, Does.Contain("Select"));
    }

    [Test]
    public void GetThemeTokens_NoFilter_ReturnsAll()
    {
        var json = ToJson(_themeTools.GetThemeTokens());
        var categories = json.EnumerateObject()
            .Select(p => (p.Name, Count: p.Value.GetArrayLength()))
            .OrderBy(x => x.Name)
            .ToList();

        Console.WriteLine($"Categories returned ({categories.Count}):");
        foreach (var (name, count) in categories)
            Console.WriteLine($"  {name,-12}: {count} tokens");

        Assert.That(json.ValueKind, Is.EqualTo(JsonValueKind.Object));
        Assert.That(categories.Count, Is.GreaterThan(0));
    }

    [Test]
    public void GetExamples_Button_ReturnsSnippet()
    {
        var result = ToJson(_docTools.GetExamples("Button"));
        var examples = result.GetProperty("examples").EnumerateArray().ToList();

        Console.WriteLine($"Component : Button");
        Console.WriteLine($"Examples  : {examples.Count}");
        foreach (var ex in examples)
        {
            // Use EnumerateObject to print whatever keys are actually present
            var props = ex.EnumerateObject().ToDictionary(p => p.Name, p => p.Value.GetString() ?? "");
            var name = props.GetValueOrDefault("Name") ?? props.GetValueOrDefault("name", "?");
            var desc = props.GetValueOrDefault("Description") ?? props.GetValueOrDefault("description", "?");
            var razor = props.GetValueOrDefault("RazorSnippet") ?? props.GetValueOrDefault("razorSnippet", "?");
            Console.WriteLine($"  [{name}] {desc}");
            Console.WriteLine($"    Razor: {razor[..Math.Min(80, razor.Length)]}...");
        }

        Assert.That(examples.Count, Is.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void GetAccessibilitySpec_Dialog_HasAriaModal()
    {
        var spec = _a11y.GetByComponent("Dialog");

        Console.WriteLine($"Component        : Dialog");
        Console.WriteLine($"ARIA role        : {spec?.AriaRole}");
        Console.WriteLine($"Required attrs   : {string.Join(", ", spec?.RequiredAttributes ?? [])}");
        Console.WriteLine($"Has aria-modal   : {spec?.RequiredAttributes.Contains("aria-modal")} (expected: true)");
        Console.WriteLine($"Keyboard map     :");
        if (spec is not null)
            foreach (var k in spec.KeyboardMap)
                Console.WriteLine($"  {k.Key,-20} → {k.Action}");

        Assert.That(spec, Is.Not.Null);
        Assert.That(spec!.RequiredAttributes, Does.Contain("aria-modal"));
    }
}
