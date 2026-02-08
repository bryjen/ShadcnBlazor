using System.Xml.Linq;

namespace ShadcnBlazor.Cli.Files;

public class CsprojUtils
{
    public static FileInfo? GetCsproj(DirectoryInfo directoryInfo)
        => directoryInfo.EnumerateFiles("*.csproj").FirstOrDefault();
    
    public static bool IsBlazorProject(string csprojPath)
    {
        var doc = XDocument.Load(csprojPath);
        var project = doc.Root;
    
        // Check SDK attribute
        var sdk = project?.Attribute("Sdk")?.Value;
    
        return sdk switch
        {
            "Microsoft.NET.Sdk.Web" => HasBlazorPackages(doc),
            "Microsoft.NET.Sdk.Razor" => true,
            "Microsoft.NET.Sdk.BlazorWebAssembly" => true,
            _ => false
        };
    }

    private static bool HasBlazorPackages(XDocument doc)
    {
        var packageRefs = doc.Descendants("PackageReference")
            .Select(p => p.Attribute("Include")?.Value)
            .Where(p => p != null);
    
        return packageRefs.Any(p => 
            p.StartsWith("Microsoft.AspNetCore.Components") ||
            p == "Microsoft.AspNetCore.Components.WebAssembly" ||
            p == "Microsoft.AspNetCore.Components.WebAssembly.Server");
    }
}