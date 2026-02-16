namespace ShadcnBlazor.Cli.Models;

public class ComponentDefinition
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public string[] Dependencies { get; init; } = [];
}
