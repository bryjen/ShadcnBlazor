using NUnit.Framework;
using ShadcnBlazor.Mcp.Services;

namespace ShadcnBlazor.Mcp.Tests.Services;

[TestFixture]
public class AccessibilitySpecReaderServiceTests
{
    private AccessibilitySpecReaderService _sut = null!;

    [SetUp]
    public void SetUp() => _sut = new AccessibilitySpecReaderService();

    [Test]
    public void GetByComponent_KnownComponent_ReturnsAriaRole()
    {
        var spec = _sut.GetByComponent("Button");

        Console.WriteLine($"Component        : Button");
        Console.WriteLine($"ARIA role        : {spec?.AriaRole ?? "(null)"}");
        Console.WriteLine($"Required attrs   : {(spec is null ? "(null)" : string.Join(", ", spec.RequiredAttributes))}");
        Console.WriteLine($"Optional attrs   : {(spec is null ? "(null)" : string.Join(", ", spec.OptionalAttributes))}");

        Assert.That(spec, Is.Not.Null);
        Assert.That(spec!.AriaRole, Is.Not.Empty);
    }

    [Test]
    public void GetByComponent_Dialog_ReturnsKeyboardMap()
    {
        var spec = _sut.GetByComponent("Dialog");

        Console.WriteLine($"Component     : Dialog");
        Console.WriteLine($"ARIA role     : {spec?.AriaRole}");
        Console.WriteLine($"Keyboard map  :");
        if (spec is not null)
            foreach (var k in spec.KeyboardMap)
                Console.WriteLine($"  {k.Key,-20} → {k.Action}");

        var hasEscape = spec?.KeyboardMap.Any(k => k.Key == "Escape") ?? false;
        Console.WriteLine($"Has Escape key: {hasEscape} (expected: true)");

        Assert.That(spec, Is.Not.Null);
        Assert.That(hasEscape, Is.True);
    }

    [Test]
    public void GetByComponent_KnownComponent_ReturnsKeyboardMap()
    {
        var spec = _sut.GetByComponent("Select");

        Console.WriteLine($"Component     : Select");
        Console.WriteLine($"Keyboard map ({spec?.KeyboardMap.Length ?? 0} entries):");
        if (spec is not null)
            foreach (var k in spec.KeyboardMap)
                Console.WriteLine($"  {k.Key,-20} → {k.Action}");

        Assert.That(spec, Is.Not.Null);
        Assert.That(spec!.KeyboardMap, Is.Not.Empty);
    }

    [Test]
    public void GetByComponent_UnknownComponent_ReturnsNull()
    {
        var spec = _sut.GetByComponent("NonExistent");

        Console.WriteLine($"Query  : \"NonExistent\"");
        Console.WriteLine($"Result : {(spec is null ? "null (expected)" : spec.ComponentName)}");

        Assert.That(spec, Is.Null);
    }

    [Test]
    public void GetByComponent_CaseInsensitive()
    {
        var lower = _sut.GetByComponent("button");
        var pascal = _sut.GetByComponent("Button");

        Console.WriteLine($"\"button\" → AriaRole: {lower?.AriaRole ?? "(null)"}");
        Console.WriteLine($"\"Button\" → AriaRole: {pascal?.AriaRole ?? "(null)"}");
        Console.WriteLine($"Roles match: {lower?.AriaRole == pascal?.AriaRole}");

        Assert.That(lower, Is.Not.Null);
        Assert.That(lower!.AriaRole, Is.EqualTo(pascal!.AriaRole));
    }
}
