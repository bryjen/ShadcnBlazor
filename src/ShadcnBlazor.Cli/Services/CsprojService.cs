using System.Xml.Linq;

#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.

namespace ShadcnBlazor.Cli.Services;

public class CsprojService
{
    public FileInfo? GetCsproj(DirectoryInfo directoryInfo)
        => directoryInfo.EnumerateFiles("*.csproj").FirstOrDefault();
    
    public BlazorProjectType? GetBlazorProjectType(string csprojPath)
    {
        var doc = XDocument.Load(csprojPath);
        var project = doc.Root;
    
        // Check SDK attribute
        var sdk = project?.Attribute("Sdk")?.Value;
    
        return sdk switch
        {
            "Microsoft.NET.Sdk.BlazorWebAssembly" => BlazorProjectType.WebAssembly,
            "Microsoft.NET.Sdk.Web" => DetermineWebProjectType(doc),
            "Microsoft.NET.Sdk.Razor" => BlazorProjectType.RazorClassLibrary,
            _ => null
        };
    }

    private static BlazorProjectType DetermineWebProjectType(XDocument doc)
    {
        var packages = doc.Descendants("PackageReference")
            .Select(p => p.Attribute("Include")?.Value)
            .Where(p => p != null)
            .Cast<string>()
            .ToList();

        // .NET 8+ Blazor Web App (explicit WebAssembly.Server package)
        if (packages.Any(p => p.Contains("Components.WebAssembly.Server")))
            return BlazorProjectType.BlazorWebApp;

        // Explicit Blazor packages present
        var hasBlazorPackages = packages.Any(p =>
            p.StartsWith("Microsoft.AspNetCore.Components") ||
            p == "Microsoft.AspNetCore.Components.WebAssembly" ||
            p == "Microsoft.AspNetCore.Components.WebAssembly.Server");

        if (hasBlazorPackages)
            return BlazorProjectType.Server;

        // No explicit Blazor packages - Blazor comes from Microsoft.NET.Sdk.Web + shared framework
        // (e.g. Blazor Server with global interactivity, Blazor Web App)
        return BlazorProjectType.BlazorWebApp;
    }
    
    public string GetRootNamespace(FileInfo csprojFile)
    {
        var doc = XDocument.Load(csprojFile.FullName);
        var rootNamespace = doc.Descendants("RootNamespace").FirstOrDefault()?.Value;
        
        return rootNamespace ?? Path.GetFileNameWithoutExtension(csprojFile.Name);
    }

    public bool HasPackageReference(FileInfo csprojFile, string packageName)
    {
        var doc = XDocument.Load(csprojFile.FullName);
        return doc.Descendants("PackageReference")
            .Any(p => p.Attribute("Include")?.Value == packageName);
    }
    
    public bool EnsurePackageReference(FileInfo csprojFile, string packageName, string version)
    {
        var doc = XDocument.Load(csprojFile.FullName);
        var root = doc.Root ?? throw new InvalidOperationException("Invalid csproj file structure.");
        
        // Check if package already exists
        var existingPackage = doc.Descendants("PackageReference")
            .FirstOrDefault(p => p.Attribute("Include")?.Value == packageName);
        
        if (existingPackage != null)
        {
            return false; // Package already exists
        }
        
        // Find or create ItemGroup for PackageReference
        var itemGroup = root.Elements("ItemGroup")
            .FirstOrDefault(ig => ig.Elements("PackageReference").Any());
        
        if (itemGroup == null)
        {
            // Create new ItemGroup if none exists
            itemGroup = new XElement("ItemGroup");
            root.Add(itemGroup);
        }
        
        // Add the package reference
        var packageRef = new XElement("PackageReference");
        packageRef.SetAttributeValue("Include", packageName);
        packageRef.SetAttributeValue("Version", version);
        itemGroup.Add(packageRef);
        
        // Save the file
        doc.Save(csprojFile.FullName);
        
        return true; // Package was added
    }
}

public enum BlazorProjectType
{
    WebAssembly,
    Server,
    BlazorWebApp,
    RazorClassLibrary
}

public static class BlazorProjectTypeExtensions
{
    public static string GetImportsPath(this BlazorProjectType projectType)
    {
        return projectType switch
        {
            BlazorProjectType.WebAssembly => "_Imports.razor",
            BlazorProjectType.Server => "Components/_Imports.razor",
            BlazorProjectType.BlazorWebApp => "Components/_Imports.razor",
            BlazorProjectType.RazorClassLibrary => "_Imports.razor",
        };
    }
    
    public static string GetAppRazorPath(this BlazorProjectType projectType)
    {
        return projectType switch
        {
            BlazorProjectType.WebAssembly => "App.razor",
            BlazorProjectType.Server => "Components/App.razor",
            BlazorProjectType.BlazorWebApp => "Components/App.razor",
            BlazorProjectType.RazorClassLibrary => "App.razor",
            _ => throw new ArgumentOutOfRangeException(nameof(projectType))
        };
    }
    
    public static string GetIndexHtmlPath(this BlazorProjectType projectType)
    {
        return projectType switch
        {
            BlazorProjectType.WebAssembly => "wwwroot/index.html",
            BlazorProjectType.Server => null!, // Uses App.razor instead
            BlazorProjectType.BlazorWebApp => null!, // Uses App.razor instead
            BlazorProjectType.RazorClassLibrary => null!,
            _ => throw new ArgumentOutOfRangeException(nameof(projectType))
        };
    }
}
