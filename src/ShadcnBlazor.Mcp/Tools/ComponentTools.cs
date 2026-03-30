using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Docs.Models;
using ModelContextProtocol.Server;
using ShadcnBlazor.Mcp.Services;
using ShadcnBlazor.Services.Models;

namespace ShadcnBlazor.Mcp.Tools;

[McpServerToolType]
public class ComponentTools(
    ComponentRegistryService registry,
    DocumentationReaderService docs,
    ApiDocumentationService apiDocs,
    SnippetExampleService snippets)
{
    [McpServerTool, Description("List all available ShadcnBlazor components with their names, descriptions, and tags.")]
    public object ListComponents()
    {
        return registry.GetAll().Select(c => new
        {
            name = c.Name,
            description = c.Description,
            tags = c.Tags.Select(t => t.ToString()).ToArray(),
            slug = c.Slug
        }).ToList();
    }

    [McpServerTool, Description("Get detailed metadata for a component by name, including its parameters and dependencies.")]
    public object GetComponent([Description("Component name (e.g. 'Button', 'Dialog')")] string name)
    {
        var comp = registry.GetByName(name);
        if (comp is null)
            return new { error = $"Component '{name}' not found. Use list_components to see available components." };

        var docType = apiDocs.Get(comp.Name);
        var parameters = GetComponentParameters(comp.Name, docType);
        var examples = MergeExamples(
            docs.GetExamples(comp.Name),
            snippets.GetComponentExamples(comp.Name)
        );

        return new
        {
            name = comp.Name,
            description = comp.Description,
            slug = comp.Slug,
            tags = comp.Tags.Select(t => t.ToString()).ToArray(),
            dependencies = comp.Dependencies,
            parameters,
            documentation = new
            {
                summary = docType?.Summary ?? string.Empty,
                methods = docType?.Methods.Select(m => new
                {
                    name = m.Name,
                    summary = m.Summary,
                    returnType = m.ReturnType,
                    returns = m.Returns,
                    isInherited = m.IsInherited
                }).ToList() ?? [],
                events = docType?.Events.Select(e => new
                {
                    name = e.Name,
                    type = e.Type,
                    summary = e.Summary,
                    isInherited = e.IsInherited
                }).ToList() ?? []
            },
            examples = examples.Select(e => new
            {
                name = e.Name,
                description = e.Description,
                razorSnippet = e.RazorSnippet
            }).ToList(),
            requiredActions = comp.RequiredActions.Select(a => a.GetType().Name).ToArray()
        };
    }

    [McpServerTool, Description("Get all dependencies for a component, including required setup actions.")]
    public object GetComponentDependencies([Description("Component name")] string name)
    {
        var comp = registry.GetByName(name);
        if (comp is null)
            return new { error = $"Component '{name}' not found." };

        var deps = registry.GetDependencies(name);
        return new
        {
            component = comp.Name,
            dependencies = deps.Select(d => new
            {
                name = d.Name,
                description = d.Description,
                requiredActions = d.RequiredActions.Select(a => a.GetType().Name).ToArray()
            }).ToList()
        };
    }

    [McpServerTool, Description("Search for components by name or description keyword.")]
    public object SearchComponents([Description("Search query")] string query)
    {
        var results = registry.Search(query);
        if (!results.Any())
            return new { results = Array.Empty<object>(), message = $"No components found matching '{query}'." };

        return new
        {
            results = results.Select(c => new
            {
                name = c.Name,
                description = c.Description,
                tags = c.Tags.Select(t => t.ToString()).ToArray()
            }).ToList()
        };
    }

    private static List<object> GetComponentParameters(string componentName, DocumentedType? docType)
    {
        var assembly = typeof(ShadcnBlazor.Components.Button.Button).Assembly;
        var type = FindComponentType(assembly, componentName);
        if (type is null) return [];

        var docProps = docType?.Properties ?? Array.Empty<DocumentedProperty>();
        var docByName = docProps.ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(p =>
            {
                docByName.TryGetValue(p.Name, out var docProp);

                var desc = p.GetCustomAttribute<DescriptionAttribute>()?.Description
                           ?? docProp?.Summary
                           ?? string.Empty;
                var category = p.GetCustomAttribute<CategoryAttribute>()?.Category.ToString()
                               ?? docProp?.Category
                               ?? "General";
                return (object)new
                {
                    name = p.Name,
                    type = GetFriendlyTypeName(p.PropertyType),
                    category,
                    description = desc,
                    remarks = docProp?.Remarks ?? string.Empty,
                    defaultValue = docProp?.DefaultValue ?? string.Empty,
                    order = docProp?.Order ?? int.MaxValue,
                    isRequired = p.GetCustomAttribute<EditorRequiredAttribute>() is not null,
                    isCascading = p.GetCustomAttribute<CascadingParameterAttribute>() is not null,
                    isEventCallback = IsEventCallback(p.PropertyType),
                    isInherited = docProp?.IsInherited ?? p.DeclaringType != type
                };
            })
            .ToList();
    }

    private static Type? FindComponentType(Assembly assembly, string componentName)
    {
        return assembly.GetExportedTypes().FirstOrDefault(t =>
            t.Name.Equals(componentName, StringComparison.OrdinalIgnoreCase) &&
            typeof(Microsoft.AspNetCore.Components.IComponent).IsAssignableFrom(t));
    }

    private static string GetFriendlyTypeName(Type type)
    {
        if (type.IsGenericType)
        {
            var genericName = type.GetGenericTypeDefinition().Name;
            genericName = genericName[..genericName.IndexOf('`')];
            var typeArgs = string.Join(", ", type.GetGenericArguments().Select(GetFriendlyTypeName));
            return $"{genericName}<{typeArgs}>";
        }

        return type.Name switch
        {
            "String" => "string",
            "Int32" => "int",
            "Int64" => "long",
            "Boolean" => "bool",
            "Double" => "double",
            "Single" => "float",
            "Decimal" => "decimal",
            "Object" => "object",
            _ => type.Name
        };
    }

    private static bool IsEventCallback(Type type)
    {
        if (type.Name == "EventCallback")
            return true;

        if (!type.IsGenericType)
            return false;

        var genericDef = type.GetGenericTypeDefinition();
        return genericDef.Name.StartsWith("EventCallback", StringComparison.Ordinal);
    }

    private static List<ComponentExample> MergeExamples(
        IEnumerable<ComponentExample> fromDocs,
        IEnumerable<ComponentExample> fromSnippets)
    {
        return fromDocs
            .Concat(fromSnippets)
            .GroupBy(e => e.RazorSnippet, StringComparer.Ordinal)
            .Select(g => g.First())
            .ToList();
    }
}
