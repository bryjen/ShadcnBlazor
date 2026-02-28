using NUnit.Framework;
using ShadcnBlazor.Mcp.Services;

namespace ShadcnBlazor.Mcp.Tests.Services;

[TestFixture]
public class DocumentationReaderServiceTests
{
    private DocumentationReaderService _sut = null!;

    [SetUp]
    public void SetUp() => _sut = new DocumentationReaderService();

    [Test]
    public void GetConventions_ReturnsNonEmptyResult()
    {
        var conventions = _sut.GetConventions();

        Console.WriteLine($"Conventions length : {conventions.Length} chars");
        Console.WriteLine($"First 120 chars    : {conventions[..Math.Min(120, conventions.Length)].Replace('\n', ' ')}...");

        Assert.That(conventions, Is.Not.Empty);
    }

    [Test]
    public void GetConventions_ContainsShadcnComponentBase()
    {
        var conventions = _sut.GetConventions();
        var idx = conventions.IndexOf("ShadcnComponentBase", StringComparison.Ordinal);

        Console.WriteLine($"\"ShadcnComponentBase\" found at index: {idx}");
        Console.WriteLine($"Surrounding context: ...{conventions[Math.Max(0, idx - 20)..Math.Min(conventions.Length, idx + 40)]}...");

        Assert.That(conventions, Does.Contain("ShadcnComponentBase"));
    }

    [Test]
    public void GetExamples_KnownComponent_ReturnsAtLeastOneExample()
    {
        var examples = _sut.GetExamples("Button");

        Console.WriteLine($"Component : Button");
        Console.WriteLine($"Examples  : {examples.Count}");
        foreach (var ex in examples)
            Console.WriteLine($"  [{ex.Name}] {ex.Description}");

        Assert.That(examples.Count, Is.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void GetExamples_BasicExampleHasRazorContent()
    {
        var examples = _sut.GetExamples("Button");
        var basic = examples.First();

        Console.WriteLine($"Example name   : {basic.Name}");
        Console.WriteLine($"Description    : {basic.Description}");
        Console.WriteLine($"Razor snippet  :");
        Console.WriteLine(basic.RazorSnippet);

        Assert.That(basic.RazorSnippet, Is.Not.Empty);
    }

    [Test]
    public void GetExamples_UnknownComponent_ReturnsEmpty()
    {
        var examples = _sut.GetExamples("NonExistent");

        Console.WriteLine($"Query  : \"NonExistent\"");
        Console.WriteLine($"Count  : {examples.Count} (expected: 0)");

        Assert.That(examples, Is.Empty);
    }

    [Test]
    public void GetExamples_CaseInsensitive()
    {
        var lower = _sut.GetExamples("button");
        var pascal = _sut.GetExamples("Button");

        Console.WriteLine($"\"button\" → {lower.Count} examples");
        Console.WriteLine($"\"Button\" → {pascal.Count} examples");
        Console.WriteLine($"Counts match: {lower.Count == pascal.Count}");

        Assert.That(lower.Count, Is.EqualTo(pascal.Count));
    }
}
