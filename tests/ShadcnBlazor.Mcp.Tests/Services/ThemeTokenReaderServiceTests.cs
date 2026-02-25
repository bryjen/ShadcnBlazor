using NUnit.Framework;
using ShadcnBlazor.Mcp.Services;

namespace ShadcnBlazor.Mcp.Tests.Services;

[TestFixture]
public class ThemeTokenReaderServiceTests
{
    private ThemeTokenReaderService _sut = null!;

    [SetUp]
    public void SetUp() => _sut = new ThemeTokenReaderService();

    [Test]
    public void GetAll_ReturnsTokensGroupedByCategory()
    {
        var all = _sut.GetAll();

        Console.WriteLine($"Categories found: {string.Join(", ", all.Keys.Order())}");
        foreach (var (cat, tokens) in all.OrderBy(k => k.Key))
            Console.WriteLine($"  {cat,-12}: {tokens.Count} tokens");

        Assert.That(all.Keys, Does.Contain("color"));
        Assert.That(all.Keys, Does.Contain("radius"));
    }

    [Test]
    public void GetAll_ColorCategoryContainsBackground()
    {
        var all = _sut.GetAll();
        all.TryGetValue("color", out var colorTokens);

        var backgroundToken = colorTokens?.FirstOrDefault(t => t.Name == "--background");

        Console.WriteLine($"Color tokens count  : {colorTokens?.Count ?? 0}");
        Console.WriteLine($"--background found  : {backgroundToken is not null}");
        Console.WriteLine($"--background value  : {backgroundToken?.Value ?? "(not found)"}");

        Assert.That(all.TryGetValue("color", out _), Is.True);
        Assert.That(backgroundToken, Is.Not.Null);
    }

    [Test]
    public void GetAll_AllTokensHaveNameAndValue()
    {
        var all = _sut.GetAll();
        var allTokens = all.Values.SelectMany(v => v).ToList();
        var invalid = allTokens.Where(t => string.IsNullOrEmpty(t.Name) || string.IsNullOrEmpty(t.Value)).ToList();

        Console.WriteLine($"Total tokens parsed : {allTokens.Count}");
        Console.WriteLine($"Invalid tokens      : {invalid.Count} (expected: 0)");
        if (invalid.Count > 0)
            foreach (var t in invalid)
                Console.WriteLine($"  BAD: name=\"{t.Name}\" value=\"{t.Value}\"");

        Assert.That(allTokens, Is.Not.Empty);
        Assert.That(invalid, Is.Empty);
    }

    [Test]
    public void GetAll_RadiusTokensPresent()
    {
        var all = _sut.GetAll();
        all.TryGetValue("radius", out var radiusTokens);

        Console.WriteLine($"Radius tokens: {(radiusTokens is null ? "(category missing)" : string.Join(", ", radiusTokens.Select(t => $"{t.Name}={t.Value}")))}");

        Assert.That(all.TryGetValue("radius", out _), Is.True);
        Assert.That(radiusTokens!, Is.Not.Empty);
    }
}
