using System.Reflection;
using ShadcnBlazor.Cli.Exception;
using ShadcnBlazor.Cli.Models;

namespace ShadcnBlazor.Cli.Services;

public class ComponentService
{
    public List<ComponentData> LoadComponents()
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        var assemblyDir = Path.GetDirectoryName(executingAssembly.Location) 
            ?? throw new InvalidOperationException("Could not determine assembly directory.");
        
        var componentsAssemblyPath = Path.Join(assemblyDir, "ShadcnBlazor.dll");
        var assembly = Assembly.LoadFrom(componentsAssemblyPath);
        
        return ComponentData.GetComponents(assembly).ToList();
    }
    
    public ComponentData FindComponent(List<ComponentData> components, string componentName)
    {
        var component = components.FirstOrDefault(c =>
            string.Equals(componentName.Trim(), c.ComponentMetadata.Name, StringComparison.InvariantCultureIgnoreCase));
        
        if (component is null)
        {
            throw new ComponentNotFoundException(componentName);
        }
        
        return component;
    }
    
    public DirectoryInfo GetComponentsSourceDirectory()
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        var assemblyDir = Path.GetDirectoryName(executingAssembly.Location) 
            ?? throw new InvalidOperationException("Could not determine assembly directory.");
        
        return new DirectoryInfo(Path.Join(assemblyDir, "components"));
    }
}
