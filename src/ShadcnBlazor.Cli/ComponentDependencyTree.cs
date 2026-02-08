using ShadcnBlazor.Cli.Models;

namespace ShadcnBlazor.Cli;

/// <summary>
/// 
/// </summary>
public class ComponentDependencyTree
{
    public required ComponentDependencyNode RootNode { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="outputProjectConfig"></param>
    /// <param name="components"></param>
    /// <param name="rootComponentName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static ComponentDependencyTree BuildComponentDependencyTree(
        OutputProjectConfig outputProjectConfig,
        IEnumerable<ComponentData> components, 
        string rootComponentName)
    {
        var componentDict = components.ToDictionary(c => 
            c.ComponentMetadata.Name.Trim().ToLower(), c => c);

        ComponentDependencyNode BuildComponentDependencyNode(string componentName)
        {
            if (!componentDict.TryGetValue(componentName, out var componentData))
                throw new ArgumentException($"Component '{componentName}' not found in the provided components.");

            var componentDependencies = componentData.ComponentMetadata.Dependencies;
            var expectedLocation = Path.Combine(outputProjectConfig.ComponentsOutputDir, componentData.ComponentMetadata.Name);
            var resolvedDependencies = componentDependencies.Select(BuildComponentDependencyNode).ToList();
            
            return new ComponentDependencyNode
            {
                Component = componentData,
                ExpectedLocation = new DirectoryInfo(expectedLocation),
                ResolvedDependencies = resolvedDependencies
            };
        }
        
        var rootNode = BuildComponentDependencyNode(rootComponentName);
        return new ComponentDependencyTree
        {
            RootNode = rootNode
        };
    }
}

/// <summary>
/// 
/// </summary>
public class ComponentDependencyNode
{
    public required ComponentData Component { get; init; }
    public required List<ComponentDependencyNode> ResolvedDependencies { get; init; }
    public required DirectoryInfo ExpectedLocation { get; init; }
}