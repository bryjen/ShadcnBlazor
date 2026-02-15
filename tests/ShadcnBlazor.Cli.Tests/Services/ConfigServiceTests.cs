using NUnit.Framework;
using ShadcnBlazor.Cli.Exception;
using ShadcnBlazor.Cli.Models;
using ShadcnBlazor.Cli.Services;

namespace ShadcnBlazor.Cli.Tests.Services;

public class ConfigServiceTests : TestBase
{
    private readonly ConfigService _sut = new();

    [Test]
    public void LoadConfig_ParsesValidYaml()
    {
        using var tempDir = new TempDirectory();
        var yaml = """
            componentsOutputDir: ./Components/Core
            servicesOutputDir: ./Services/Components
            rootNamespace: MyApp
            """;
        File.WriteAllText(Path.Combine(tempDir.Path, "shadcn-blazor.yaml"), yaml);
        Console.WriteLine($"  Input YAML: componentsOutputDir, servicesOutputDir, rootNamespace");

        var config = _sut.LoadConfig(new DirectoryInfo(tempDir.Path));

        Console.WriteLine($"  Actual: ComponentsOutputDir={config.ComponentsOutputDir}, ServicesOutputDir={config.ServicesOutputDir}, RootNamespace={config.RootNamespace}");
        Console.WriteLine($"  Expected: ./Components/Core, ./Services/Components, MyApp");
        Assert.That(config.ComponentsOutputDir, Is.EqualTo("./Components/Core"));
        Assert.That(config.ServicesOutputDir, Is.EqualTo("./Services/Components"));
        Assert.That(config.RootNamespace, Is.EqualTo("MyApp"));
    }

    [Test]
    public void LoadConfig_ThrowsWhenFileMissing()
    {
        using var tempDir = new TempDirectory();
        Console.WriteLine($"  Input: empty directory (no shadcn-blazor.yaml)");
        Console.WriteLine($"  Expected: ConfigFileNotFoundException");

        var ex = Assert.Throws<ConfigFileNotFoundException>(() =>
            _sut.LoadConfig(new DirectoryInfo(tempDir.Path)));

        Console.WriteLine($"  Actual: threw {ex?.GetType().Name}: {ex?.Message}");
    }

    [Test]
    public void SaveConfig_AndLoadConfig_RoundTrips()
    {
        using var tempDir = new TempDirectory();
        var original = new OutputProjectConfig
        {
            ComponentsOutputDir = "./Components/Core",
            ServicesOutputDir = "./Services/Components",
            RootNamespace = "TestProject"
        };
        Console.WriteLine($"  Input config: ComponentsOutputDir={original.ComponentsOutputDir}, RootNamespace={original.RootNamespace}");

        _sut.SaveConfig(new DirectoryInfo(tempDir.Path), original);
        var loaded = _sut.LoadConfig(new DirectoryInfo(tempDir.Path));

        Console.WriteLine($"  Actual loaded: ComponentsOutputDir={loaded.ComponentsOutputDir}, RootNamespace={loaded.RootNamespace}");
        Console.WriteLine($"  Expected: match original (round-trip)");
        Assert.That(loaded.ComponentsOutputDir, Is.EqualTo(original.ComponentsOutputDir));
        Assert.That(loaded.ServicesOutputDir, Is.EqualTo(original.ServicesOutputDir));
        Assert.That(loaded.RootNamespace, Is.EqualTo(original.RootNamespace));
    }

    private sealed class TempDirectory : IDisposable
    {
        public string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        public TempDirectory()
        {
            Directory.CreateDirectory(Path);
        }

        public void Dispose() => Directory.Delete(Path, recursive: true);
    }
}
