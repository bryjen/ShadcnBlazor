using ShadcnBlazor.Services;
using ShadcnBlazor.Services.Models;

namespace ShadcnBlazor.Mcp.Services;

public class ComponentRegistryService
{
    private readonly ComponentDefinition[] _all = ComponentRegistry.AllComponents;

    public IReadOnlyList<ComponentDefinition> GetAll() => _all;

    public ComponentDefinition? GetByName(string name) =>
        _all.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public IReadOnlyList<ComponentDefinition> GetDependencies(string name)
    {
        var comp = GetByName(name);
        if (comp is null) return [];
        return comp.Dependencies
            .Select(d => GetByName(d))
            .OfType<ComponentDefinition>()
            .ToList();
    }

    public IReadOnlyList<ComponentDefinition> Search(string query) =>
        _all.Where(c =>
            c.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            c.Description.Contains(query, StringComparison.OrdinalIgnoreCase)
        ).ToList();
}
