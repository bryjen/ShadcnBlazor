using Spectre.Console;

namespace ShadcnBlazor.Cli.Models;

public class ComponentDependencyTree
{
    public required ComponentDependencyNode RootNode { get; set; }

    public static ComponentDependencyTree BuildComponentDependencyTree(
        OutputProjectConfig outputProjectConfig,
        IEnumerable<ComponentDefinition> components,
        string rootComponentName)
    {
        var componentDict = components.ToDictionary(c =>
            c.Name.Trim().ToLower(), c => c);

        ComponentDependencyNode BuildComponentDependencyNode(string componentName)
        {
            var key = componentName.Trim().ToLower();
            if (!componentDict.TryGetValue(key, out var definition))
                throw new ArgumentException($"Component '{componentName}' not found in the provided components.");

            var expectedLocation = Path.Combine(outputProjectConfig.ComponentsOutputDir, definition.Name);
            var resolvedDependencies = definition.Dependencies.Select(BuildComponentDependencyNode).ToList();

            return new ComponentDependencyNode
            {
                Component = definition,
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

    public Tree AsSpectreConsoleTree()
    {
        var name = RootNode.Component.Name;
        var tree = new Tree(name);
        var treeInterface = (IHasTreeNodes)tree;

        void AddNodeRecursively(List<ComponentDependencyNode> children, IHasTreeNodes printNode)
        {
            foreach (var child in children)
            {
                var nodeName = child.Component.Name;
                var subNode = printNode.AddNode(nodeName);
                AddNodeRecursively(child.ResolvedDependencies, subNode);
            }
        }

        AddNodeRecursively(RootNode.ResolvedDependencies, treeInterface);
        return tree;
    }
}

public class ComponentDependencyNode
{
    public required ComponentDefinition Component { get; init; }
    public required List<ComponentDependencyNode> ResolvedDependencies { get; init; }
    public required DirectoryInfo ExpectedLocation { get; init; }
}
