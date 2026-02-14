using System.Reflection;
using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Shared.Attributes;

namespace ShadcnBlazor.Docs.Services;

/// <summary>
/// Discovers and tracks components from the ShadcnBlazor assembly that have ComponentMetadataAttribute.
/// Mirrors the CLI's component discovery logic.
/// </summary>
public class ComponentRegistryService
{
    private readonly Lazy<IReadOnlyList<ComponentRegistryEntry>> _components;

    public ComponentRegistryService()
    {
        _components = new Lazy<IReadOnlyList<ComponentRegistryEntry>>(DiscoverComponents);
    }

    /// <summary>
    /// All components with ComponentMetadataAttribute, ordered by name.
    /// </summary>
    public IReadOnlyList<ComponentRegistryEntry> Components => _components.Value;

    private static readonly IReadOnlyList<ComponentRegistryEntry> PseudoComponents =
    [
        new ComponentRegistryEntry("Icons", "icons"),
        new ComponentRegistryEntry("Typography", "typography"),
    ];

    private static IReadOnlyList<ComponentRegistryEntry> DiscoverComponents()
    {
        var assembly = typeof(ShadcnBlazor.Components.Button.Button).Assembly;

        var libraryComponents = assembly.GetTypes()
            .Where(t => t.IsPublic && !t.IsAbstract)
            .Where(t => typeof(ComponentBase).IsAssignableFrom(t))
            .Where(t => t.GetCustomAttribute<ComponentMetadataAttribute>() is not null)
            .Select(t =>
            {
                var metadata = t.GetCustomAttribute<ComponentMetadataAttribute>()!;
                return new ComponentRegistryEntry(metadata.Name, ToSlug(metadata.Name));
            });

        return libraryComponents
            .Concat(PseudoComponents)
            .OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string ToSlug(string name) => name.ToLowerInvariant();
}

public record ComponentRegistryEntry(string Name, string Slug);
