using NUnit.Framework;
using ShadcnBlazor.Cli.Services;

namespace ShadcnBlazor.Cli.Tests.Services;

public class NamespaceServiceTests : TestBase
{
    private readonly NamespaceService _sut = new();

    [Test]
    public void ReplaceNamespaceInRazor_ReplacesExistingNamespace()
    {
        var content = "@namespace ShadcnBlazor.Components.Button\n@code { }";
        var newNamespace = "MyApp.Components.Core.Button";
        Console.WriteLine($"  Input: {content.Replace("\n", "\\n")}");
        Console.WriteLine($"  newNamespace: {newNamespace}");

        var result = _sut.ReplaceNamespaceInRazor(content, newNamespace);

        Console.WriteLine($"  Actual result: {result.Replace("\n", "\\n")}");
        Console.WriteLine($"  Expected: contain '@namespace {newNamespace}', not contain 'ShadcnBlazor.Components.Button'");
        Assert.That(result, Does.Contain("@namespace MyApp.Components.Core.Button"));
        Assert.That(result, Does.Not.Contain("ShadcnBlazor.Components.Button"));
    }

    [Test]
    public void ReplaceNamespaceInRazor_AddsNamespaceWhenMissing()
    {
        var content = "@code { }";
        var newNamespace = "MyApp.Components.Core.Button";
        Console.WriteLine($"  Input: {content}");
        Console.WriteLine($"  newNamespace: {newNamespace}");

        var result = _sut.ReplaceNamespaceInRazor(content, newNamespace);

        Console.WriteLine($"  Actual result: {result.Replace("\n", "\\n")}");
        Console.WriteLine($"  Expected: start with '@namespace {newNamespace}', preserve '@code {{ }}'");
        Assert.That(result, Does.StartWith("@namespace MyApp.Components.Core.Button"));
        Assert.That(result, Does.Contain("@code { }"));
    }

    [Test]
    public void ReplaceNamespaceInRazor_RemovesNamespaceWhenNull()
    {
        var content = "@namespace ShadcnBlazor.Components.Button\n@code { }";
        Console.WriteLine($"  Input: {content.Replace("\n", "\\n")}");
        Console.WriteLine($"  newNamespace: null (remove)");

        var result = _sut.ReplaceNamespaceInRazor(content, null);

        Console.WriteLine($"  Actual result: {result.Replace("\n", "\\n")}");
        Console.WriteLine($"  Expected: no @namespace directive");
        Assert.That(result, Does.Not.Contain("@namespace"));
    }

    [Test]
    public void ReplaceNamespaceInCs_ReplacesFileScopedNamespace()
    {
        var content = "using System;\nnamespace ShadcnBlazor.Shared;\npublic class Foo { }";
        var newNamespace = "MyApp.Shared";
        Console.WriteLine($"  Input: file-scoped namespace ShadcnBlazor.Shared");
        Console.WriteLine($"  newNamespace: {newNamespace}");

        var result = _sut.ReplaceNamespaceInCs(content, newNamespace);

        Console.WriteLine($"  Actual result: {result.Replace("\n", " | ")}");
        Console.WriteLine($"  Expected: 'namespace {newNamespace};', no 'ShadcnBlazor.Shared'");
        Assert.That(result, Does.Contain("namespace MyApp.Shared;"));
        Assert.That(result, Does.Not.Contain("namespace ShadcnBlazor.Shared;"));
    }

    [Test]
    public void ReplaceNamespaceInCs_ReplacesBlockScopedNamespace()
    {
        var content = "using System;\nnamespace ShadcnBlazor.Shared\n{\n    public class Foo { }\n}";
        var newNamespace = "MyApp.Shared";
        Console.WriteLine($"  Input: block-scoped namespace ShadcnBlazor.Shared");
        Console.WriteLine($"  newNamespace: {newNamespace}");

        var result = _sut.ReplaceNamespaceInCs(content, newNamespace);

        Console.WriteLine($"  Actual result: {result.Replace("\n", " | ")}");
        Console.WriteLine($"  Expected: 'namespace {newNamespace} {{', no 'ShadcnBlazor.Shared'");
        Assert.That(result, Does.Contain("namespace MyApp.Shared {"));
        Assert.That(result, Does.Not.Contain("namespace ShadcnBlazor.Shared"));
    }

    [Test]
    public void ReplaceNamespaceInCs_AddsNamespaceWhenMissing()
    {
        var content = "using System;\npublic class Foo { }";
        var newNamespace = "MyApp.Shared";
        Console.WriteLine($"  Input: no namespace (bare class)");
        Console.WriteLine($"  newNamespace: {newNamespace}");

        var result = _sut.ReplaceNamespaceInCs(content, newNamespace);

        Console.WriteLine($"  Actual result: {result.Replace("\n", " | ")}");
        Console.WriteLine($"  Expected: contain 'namespace {newNamespace};'");
        Assert.That(result, Does.Contain("namespace MyApp.Shared;"));
    }
}
