using System.Reflection;
using ShadcnBlazor.Cli.Exception;
using ShadcnBlazor.Cli.Models;
using ShadcnBlazor.Cli.Models.Components;

namespace ShadcnBlazor.Cli.Services.Components;

public class ComponentService
{
    public List<ComponentDefinition> LoadComponents()
    {
        return ComponentRegistry.AllComponents.ToList();
    }

    public ComponentDefinition FindComponent(List<ComponentDefinition> components, string componentName)
    {
        var component = components.FirstOrDefault(c =>
            string.Equals(componentName.Trim(), c.Name, StringComparison.InvariantCultureIgnoreCase));

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

        return new DirectoryInfo(Path.Join(assemblyDir, "Components"));
    }
}
