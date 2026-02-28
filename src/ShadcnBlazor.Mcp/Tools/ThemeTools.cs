using System.ComponentModel;
using ModelContextProtocol.Server;
using ShadcnBlazor.Mcp.Services;

namespace ShadcnBlazor.Mcp.Tools;

[McpServerToolType]
public class ThemeTools(ThemeTokenReaderService themeService)
{
    [McpServerTool, Description("Get CSS theme tokens, optionally filtered by category (color, radius, shadow, typography, spacing, extended).")]
    public object GetThemeTokens([Description("Optional category filter: color, radius, shadow, typography, spacing, extended")] string? category = null)
    {
        var all = themeService.GetAll();

        if (category is not null)
        {
            if (!all.TryGetValue(category.ToLowerInvariant(), out var filtered))
                return new { error = $"Category '{category}' not found. Available: {string.Join(", ", all.Keys)}" };

            return new { category, tokens = filtered };
        }

        return all.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Select(t => new { t.Name, t.Value }).ToList()
        );
    }
}
