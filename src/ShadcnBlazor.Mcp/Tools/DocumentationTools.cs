using System.ComponentModel;
using ModelContextProtocol.Server;
using ShadcnBlazor.Mcp.Services;

namespace ShadcnBlazor.Mcp.Tools;

[McpServerToolType]
public class DocumentationTools(DocumentationReaderService docService)
{
    [McpServerTool, Description("Get ShadcnBlazor conventions: base class usage, parameter categories, two-way binding patterns, required providers, and CSS utilities.")]
    public string GetConventions() => docService.GetConventions();

    [McpServerTool, Description("Get Razor usage examples for a specific ShadcnBlazor component.")]
    public object GetExamples([Description("Component name (e.g. 'Button', 'Dialog')")] string componentName)
    {
        var examples = docService.GetExamples(componentName);
        if (!examples.Any())
            return new { message = $"No examples found for '{componentName}'.", examples = Array.Empty<object>() };

        return new { componentName, examples };
    }
}
