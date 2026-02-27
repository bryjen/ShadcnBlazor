using System.ComponentModel;
using ModelContextProtocol.Server;
using ShadcnBlazor.Mcp.Services;

namespace ShadcnBlazor.Mcp.Tools;

[McpServerToolType]
public class AccessibilityTools(AccessibilitySpecReaderService a11yService)
{
    [McpServerTool, Description("Get WAI-ARIA accessibility specification for a ShadcnBlazor component, including roles, required attributes, and keyboard interactions.")]
    public object GetAccessibilitySpec([Description("Component name (e.g. 'Button', 'Dialog', 'Select')")] string componentName)
    {
        var spec = a11yService.GetByComponent(componentName);
        if (spec is null)
            return new { error = $"No accessibility spec found for '{componentName}'." };

        return spec;
    }
}
