using System.Xml.Linq;

#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.

namespace ShadcnBlazor.Cli.Files;

public class CsprojUtils
{
    public static FileInfo? GetCsproj(DirectoryInfo directoryInfo)
        => directoryInfo.EnumerateFiles("*.csproj").FirstOrDefault();
    
    public static BlazorProjectType? GetBlazorProjectType(string csprojPath)
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

    private static BlazorProjectType? DetermineWebProjectType(XDocument doc)
    {
        var packages = doc.Descendants("PackageReference")
            .Select(p => p.Attribute("Include")?.Value)
            .Where(p => p != null)
            .ToList();
    
        // Check for Blazor packages
        var hasBlazorPackages = packages.Any(p => 
            p.StartsWith("Microsoft.AspNetCore.Components") ||
            p == "Microsoft.AspNetCore.Components.WebAssembly" ||
            p == "Microsoft.AspNetCore.Components.WebAssembly.Server");
        
        if (!hasBlazorPackages)
            return null;
        
        // .NET 8+ Blazor Web App
        if (packages.Any(p => p.Contains("Components.WebAssembly.Server")))
            return BlazorProjectType.BlazorWebApp;
        
        // Blazor Server
        return BlazorProjectType.Server;
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