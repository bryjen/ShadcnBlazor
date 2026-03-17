using NUnit.Framework;
using ShadcnBlazor.Cli.Services;

namespace ShadcnBlazor.Cli.Tests.Services;

public class UsingServiceTests : TestBase
{
    private readonly UsingService _sut = new();

    [Test]
    public void ReplaceUsingsInRazor_ReplacesNamespacePrefix()
    {
        var content = "@using ShadcnBlazor.Components.Shared\n@using ShadcnBlazor.Components.Button";
        var oldPrefix = "ShadcnBlazor";
        var newPrefix = "MyApp";
        Console.WriteLine($"  Input: {content.Replace("\n", " | ")}");
        Console.WriteLine($"  oldPrefix: {oldPrefix}, newPrefix: {newPrefix}");

        var result = _sut.ReplaceUsingsInRazor(content, oldPrefix, newPrefix);

        Console.WriteLine($"  Actual result: {result.Replace("\n", " | ")}");
        Console.WriteLine($"  Expected: @using MyApp.Components.Shared, @using MyApp.Components.Button, no @using ShadcnBlazor");
        Assert.That(result, Does.Contain("@using MyApp.Components.Shared"));
        Assert.That(result, Does.Contain("@using MyApp.Components.Button"));
        Assert.That(result, Does.Not.Contain("@using ShadcnBlazor"));
    }

    [Test]
    public void ReplaceUsingsInRazor_HandlesMultipleUsings()
    {
        var content = "@using ShadcnBlazor.Components.Shared\n@using ShadcnBlazor.Components.Shared.Enums";
        var oldPrefix = "ShadcnBlazor";
        var newPrefix = "MyApp";
        Console.WriteLine($"  Input: {content.Replace("\n", " | ")}");
        Console.WriteLine($"  oldPrefix: {oldPrefix}, newPrefix: {newPrefix}");

        var result = _sut.ReplaceUsingsInRazor(content, oldPrefix, newPrefix);

        Console.WriteLine($"  Actual result: {result.Replace("\n", " | ")}");
        Console.WriteLine($"  Expected: both usings replaced with MyApp.*");
        Assert.That(result, Does.Contain("@using MyApp.Components.Shared"));
        Assert.That(result, Does.Contain("@using MyApp.Components.Shared.Enums"));
    }

    [Test]
    public void ReplaceUsingsInCs_ReplacesNamespacePrefix()
    {
        var content = "using ShadcnBlazor.Components.Shared;\nusing ShadcnBlazor.Components.Button;";
        var oldPrefix = "ShadcnBlazor";
        var newPrefix = "MyApp";
        Console.WriteLine($"  Input: {content.Replace("\n", " | ")}");
        Console.WriteLine($"  oldPrefix: {oldPrefix}, newPrefix: {newPrefix}");

        var result = _sut.ReplaceUsingsInCs(content, oldPrefix, newPrefix);

        Console.WriteLine($"  Actual result: {result.Replace("\n", " | ")}");
        Console.WriteLine($"  Expected: using MyApp.Components.Shared;, using MyApp.Components.Button;, no using ShadcnBlazor");
        Assert.That(result, Does.Contain("using MyApp.Components.Shared;"));
        Assert.That(result, Does.Contain("using MyApp.Components.Button;"));
        Assert.That(result, Does.Not.Contain("using ShadcnBlazor"));
    }

    [Test]
    public void ReplaceUsingsInCs_HandlesNestedNamespaces()
    {
        var content = "using ShadcnBlazor.Components.Shared.Attributes;";
        var oldPrefix = "ShadcnBlazor";
        var newPrefix = "MyApp";
        Console.WriteLine($"  Input: {content}");
        Console.WriteLine($"  oldPrefix: {oldPrefix}, newPrefix: {newPrefix}");

        var result = _sut.ReplaceUsingsInCs(content, oldPrefix, newPrefix);

        Console.WriteLine($"  Actual result: {result}");
        Console.WriteLine($"  Expected: using MyApp.Components.Shared.Attributes;");
        Assert.That(result, Does.Contain("using MyApp.Components.Shared.Attributes;"));
    }
}
