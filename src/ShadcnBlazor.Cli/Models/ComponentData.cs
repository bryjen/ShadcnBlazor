using System.Reflection;
using ShadcnBlazor.ComponentDependencies;

namespace ShadcnBlazor.Cli.Models;

public record ComponentData(
    Type Type,
    ComponentMetadataAttribute ComponentMetadata,
    string Namespace,
    string FullName,
    DirectoryInfo Location)
{
    public static IEnumerable<ComponentData> GetComponents(Assembly assembly)
    {
        return assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<ComponentMetadataAttribute>() is not null)
            .Select(t => new ComponentData(
                Type: t, 
                ComponentMetadata: t.GetCustomAttribute<ComponentMetadataAttribute>()!, 
                Namespace: t.Namespace!, 
                FullName: t.FullName!, 
                Location: new DirectoryInfo(assembly.Location)));
    }
}
