using ShadcnBlazor.Services;
using ShadcnBlazor.Services.Models;

namespace ShadcnBlazor.Docs.Services;

/// <summary>
/// Metadata for a component (Name, Description). Used by docs pages.
/// </summary>
public record ComponentMetadata(string Name, string Description);

/// <summary>
/// Docs view over the core component registry. Uses core registry directly; adds docs-only pseudo-components.
/// </summary>
public class ComponentRegistryService
{
    private static readonly ComponentDefinition[] PseudoComponents =
    [
        new() { Name = "Icons", Description = "Icon usage with Lucide or other icon libraries." },
        new() { Name = "Typography", Description = "Text styling and typography utilities." },
    ];

    private static readonly HashSet<string> ComponentsWithoutOwnPage = ["ComposableTextArea", "DataTableColumn", "Shared"];

    private static IReadOnlyList<ComponentDefinition> ComponentsList { get; } =
        ComponentRegistry.AllComponents
            .Where(c => !ComponentsWithoutOwnPage.Contains(c.Name))
            .Concat(PseudoComponents)
            .OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

    /// <summary>
    /// Gets metadata for a component by name.
    /// </summary>
    public static ComponentMetadata GetMetadata(string componentName)
    {
        var def = ComponentRegistry.AllComponents
            .Concat(PseudoComponents)
            .FirstOrDefault(c => string.Equals(c.Name, componentName, StringComparison.OrdinalIgnoreCase));
        return def != null
            ? new ComponentMetadata(def.Name, def.Description)
            : new ComponentMetadata(componentName, "");
    }

    /// <summary>
    /// All components for docs (core + pseudo), ordered by name.
    /// </summary>
    public IReadOnlyList<ComponentDefinition> Components => ComponentsList;
}
