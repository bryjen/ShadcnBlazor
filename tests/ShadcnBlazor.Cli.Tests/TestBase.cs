using NUnit.Framework;

namespace ShadcnBlazor.Cli.Tests;

public abstract class TestBase
{
    [SetUp]
    public void PrintTestName()
    {
        var name = TestContext.CurrentContext.Test.Name;
        Console.WriteLine($"[TEST] {name}");
    }
}
