using NUnit.Framework;
using ShadcnBlazor.Mcp.Services;
using ShadcnBlazor.Services;

namespace ShadcnBlazor.Mcp.Tests.Services;

[TestFixture]
public class ComponentRegistryServiceTests
{
    private ComponentRegistryService _sut = null!;

    [SetUp]
    public void SetUp() => _sut = new ComponentRegistryService();

    [Test]
    public void GetAll_ReturnsAllRegisteredComponents()
    {
        var all = _sut.GetAll();
        var expected = ComponentRegistry.AllComponents.Length;
        var actual = all.Count;

        Console.WriteLine($"Expected component count : {expected}");
        Console.WriteLine($"Actual component count   : {actual}");
        Console.WriteLine($"Components: {string.Join(", ", all.Select(c => c.Name))}");

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetByName_KnownComponent_ReturnsMetadata()
    {
        var comp = _sut.GetByName("Button");

        Console.WriteLine($"Query        : \"Button\"");
        Console.WriteLine($"Name         : {comp?.Name}");
        Console.WriteLine($"Description  : {comp?.Description}");
        Console.WriteLine($"Slug         : {comp?.Slug}");

        Assert.That(comp, Is.Not.Null);
        Assert.That(comp!.Description, Is.Not.Empty);
    }

    [Test]
    public void GetByName_UnknownComponent_ReturnsNull()
    {
        var comp = _sut.GetByName("NonExistent");

        Console.WriteLine($"Query  : \"NonExistent\"");
        Console.WriteLine($"Result : {(comp is null ? "null (expected)" : comp.Name)}");

        Assert.That(comp, Is.Null);
    }

    [Test]
    public void GetByName_CaseInsensitive()
    {
        var lower = _sut.GetByName("button");
        var pascal = _sut.GetByName("Button");

        Console.WriteLine($"Query \"button\" → Name: {lower?.Name}");
        Console.WriteLine($"Query \"Button\" → Name: {pascal?.Name}");
        Console.WriteLine($"Both resolve to same component: {lower?.Name == pascal?.Name}");

        Assert.That(lower, Is.Not.Null);
        Assert.That(lower!.Name, Is.EqualTo(pascal!.Name));
    }

    [Test]
    public void GetDependencies_ComponentWithDeps_ReturnsCorrectTree()
    {
        var deps = _sut.GetDependencies("Combobox");
        var names = deps.Select(d => d.Name).ToList();

        Console.WriteLine($"Component    : Combobox");
        Console.WriteLine($"Dependencies : {string.Join(", ", names)}");
        Console.WriteLine($"Contains Popover : {names.Contains("Popover")}");
        Console.WriteLine($"Contains Shared  : {names.Contains("Shared")}");

        Assert.That(names, Does.Contain("Popover"));
        Assert.That(names, Does.Contain("Shared"));
    }

    [Test]
    public void GetDependencies_ComponentWithNoDeps_ReturnsOnlyShared()
    {
        var deps = _sut.GetDependencies("Button");
        var names = deps.Select(d => d.Name).ToList();

        Console.WriteLine($"Component    : Button");
        Console.WriteLine($"Dependencies : {string.Join(", ", names)}");
        Console.WriteLine($"Expected     : [Shared] (count: 1)");
        Console.WriteLine($"Actual count : {names.Count}");

        Assert.That(names, Does.Contain("Shared"));
        Assert.That(names, Has.Count.EqualTo(1));
    }

    [Test]
    public void Search_MatchingQuery_ReturnsRelevantComponents()
    {
        var results = _sut.Search("popover");
        var names = results.Select(c => c.Name).ToList();

        Console.WriteLine($"Query   : \"popover\"");
        Console.WriteLine($"Matches : {string.Join(", ", names)}");

        Assert.That(names, Does.Contain("Popover"));
    }

    [Test]
    public void Search_NoMatch_ReturnsEmpty()
    {
        var results = _sut.Search("xyzzy");

        Console.WriteLine($"Query   : \"xyzzy\"");
        Console.WriteLine($"Matches : {results.Count} (expected: 0)");

        Assert.That(results, Is.Empty);
    }
}
