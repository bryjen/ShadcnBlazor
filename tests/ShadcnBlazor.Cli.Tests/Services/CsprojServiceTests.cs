using NUnit.Framework;
using ShadcnBlazor.Cli.Services;

namespace ShadcnBlazor.Cli.Tests.Services;

public class CsprojServiceTests : TestBase
{
    private readonly CsprojService _sut = new();

    [Test]
    public void GetBlazorProjectType_ReturnsWebAssembly_ForBlazorWebAssemblySdk()
    {
        var csprojPath = CreateTempCsproj("""
            <Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">
              <PropertyGroup>
                <TargetFramework>net9.0</TargetFramework>
              </PropertyGroup>
            </Project>
            """);
        try
        {
            Console.WriteLine($"  Input: Sdk=Microsoft.NET.Sdk.BlazorWebAssembly");
            var result = _sut.GetBlazorProjectType(csprojPath);
            Console.WriteLine($"  Actual: {result}");
            Console.WriteLine($"  Expected: WebAssembly");
            Assert.That(result, Is.EqualTo(BlazorProjectType.WebAssembly));
        }
        finally
        {
            File.Delete(csprojPath);
        }
    }

    [Test]
    public void GetBlazorProjectType_ReturnsServer_ForWebSdkWithBlazorPackages()
    {
        var csprojPath = CreateTempCsproj("""
            <Project Sdk="Microsoft.NET.Sdk.Web">
              <PropertyGroup>
                <TargetFramework>net9.0</TargetFramework>
              </PropertyGroup>
              <ItemGroup>
                <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="9.0.0" />
              </ItemGroup>
            </Project>
            """);
        try
        {
            Console.WriteLine($"  Input: Sdk=Microsoft.NET.Sdk.Web + Components.WebAssembly (no Server)");
            var result = _sut.GetBlazorProjectType(csprojPath);
            Console.WriteLine($"  Actual: {result}");
            Console.WriteLine($"  Expected: Server");
            Assert.That(result, Is.EqualTo(BlazorProjectType.Server));
        }
        finally
        {
            File.Delete(csprojPath);
        }
    }

    [Test]
    public void GetBlazorProjectType_ReturnsBlazorWebApp_ForWebSdkWithWebAssemblyServer()
    {
        var csprojPath = CreateTempCsproj("""
            <Project Sdk="Microsoft.NET.Sdk.Web">
              <PropertyGroup>
                <TargetFramework>net9.0</TargetFramework>
              </PropertyGroup>
              <ItemGroup>
                <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.Server" Version="9.0.0" />
              </ItemGroup>
            </Project>
            """);
        try
        {
            Console.WriteLine($"  Input: Sdk=Microsoft.NET.Sdk.Web + Components.WebAssembly.Server");
            var result = _sut.GetBlazorProjectType(csprojPath);
            Console.WriteLine($"  Actual: {result}");
            Console.WriteLine($"  Expected: BlazorWebApp");
            Assert.That(result, Is.EqualTo(BlazorProjectType.BlazorWebApp));
        }
        finally
        {
            File.Delete(csprojPath);
        }
    }

    [Test]
    public void GetBlazorProjectType_ReturnsNull_ForNonBlazorProject()
    {
        var csprojPath = CreateTempCsproj("""
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net9.0</TargetFramework>
              </PropertyGroup>
            </Project>
            """);
        try
        {
            Console.WriteLine($"  Input: Sdk=Microsoft.NET.Sdk (no Blazor)");
            var result = _sut.GetBlazorProjectType(csprojPath);
            Console.WriteLine($"  Actual: {result}");
            Console.WriteLine($"  Expected: null");
            Assert.That(result, Is.Null);
        }
        finally
        {
            File.Delete(csprojPath);
        }
    }

    [Test]
    public void GetRootNamespace_ReturnsRootNamespace_WhenPresent()
    {
        var csprojPath = CreateTempCsproj("""
            <Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">
              <PropertyGroup>
                <TargetFramework>net9.0</TargetFramework>
                <RootNamespace>MyCustomNamespace</RootNamespace>
              </PropertyGroup>
            </Project>
            """);
        try
        {
            Console.WriteLine($"  Input: RootNamespace=MyCustomNamespace in csproj");
            var result = _sut.GetRootNamespace(new FileInfo(csprojPath));
            Console.WriteLine($"  Actual: {result}");
            Console.WriteLine($"  Expected: MyCustomNamespace");
            Assert.That(result, Is.EqualTo("MyCustomNamespace"));
        }
        finally
        {
            File.Delete(csprojPath);
        }
    }

    [Test]
    public void GetRootNamespace_ReturnsProjectFileName_WhenRootNamespaceAbsent()
    {
        var csprojPath = CreateTempCsproj("""
            <Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">
              <PropertyGroup>
                <TargetFramework>net9.0</TargetFramework>
              </PropertyGroup>
            </Project>
            """);
        try
        {
            var fileName = Path.GetFileName(csprojPath);
            var expectedName = Path.GetFileNameWithoutExtension(fileName);
            Console.WriteLine($"  Input: no RootNamespace, file={fileName}");
            var result = _sut.GetRootNamespace(new FileInfo(csprojPath));
            Console.WriteLine($"  Actual: {result}");
            Console.WriteLine($"  Expected: {expectedName} (from filename)");
            Assert.That(result, Is.EqualTo(expectedName));
        }
        finally
        {
            File.Delete(csprojPath);
        }
    }

    [Test]
    public void EnsurePackageReference_AddsPackage_WhenMissing()
    {
        var csprojPath = CreateTempCsproj("""
            <Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">
              <PropertyGroup>
                <TargetFramework>net9.0</TargetFramework>
              </PropertyGroup>
              <ItemGroup>
                <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="9.0.0" />
              </ItemGroup>
            </Project>
            """);
        try
        {
            Console.WriteLine($"  Input: csproj without TailwindMerge.NET");
            var result = _sut.EnsurePackageReference(new FileInfo(csprojPath), "TailwindMerge.NET", "1.2.0");
            Console.WriteLine($"  Actual return: {result}");
            Console.WriteLine($"  Expected: true (package was added)");

            var content = File.ReadAllText(csprojPath);
            var hasPackage = content.Contains("TailwindMerge.NET");
            var hasVersion = content.Contains("1.2.0");
            Console.WriteLine($"  csproj contains TailwindMerge.NET: {hasPackage}, contains 1.2.0: {hasVersion}");
            Assert.That(result, Is.True);
            Assert.That(content, Does.Contain("TailwindMerge.NET"));
            Assert.That(content, Does.Contain("1.2.0"));
        }
        finally
        {
            File.Delete(csprojPath);
        }
    }

    [Test]
    public void EnsurePackageReference_ReturnsFalse_WhenPackageAlreadyExists()
    {
        var csprojPath = CreateTempCsproj("""
            <Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">
              <PropertyGroup>
                <TargetFramework>net9.0</TargetFramework>
              </PropertyGroup>
              <ItemGroup>
                <PackageReference Include="TailwindMerge.NET" Version="1.2.0" />
              </ItemGroup>
            </Project>
            """);
        try
        {
            Console.WriteLine($"  Input: csproj already has TailwindMerge.NET");
            var result = _sut.EnsurePackageReference(new FileInfo(csprojPath), "TailwindMerge.NET", "1.2.0");
            Console.WriteLine($"  Actual return: {result}");
            Console.WriteLine($"  Expected: false (no change)");

            var content = File.ReadAllText(csprojPath);
            var count = System.Text.RegularExpressions.Regex.Matches(content, "TailwindMerge.NET").Count;
            Console.WriteLine($"  TailwindMerge.NET occurrence count: {count} (expected: 1, no duplicate)");
            Assert.That(result, Is.False);
            Assert.That(count, Is.EqualTo(1));
        }
        finally
        {
            File.Delete(csprojPath);
        }
    }

    private static string CreateTempCsproj(string content)
    {
        var path = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid():N}.csproj");
        File.WriteAllText(path, content);
        return path;
    }
}
