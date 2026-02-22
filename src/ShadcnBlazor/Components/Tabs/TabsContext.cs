using Microsoft.AspNetCore.Components;

namespace ShadcnBlazor.Components.Tabs;

internal class TabsContext
{
    public string? ActiveValue { get; set; }
    public string Orientation { get; set; } = "horizontal";
    public List<string> RegisteredTriggers { get; } = [];
    public Dictionary<string, ElementReference> TriggerRefs { get; } = [];
    public Func<string, Task>? OnTabSelected { get; set; }
}
