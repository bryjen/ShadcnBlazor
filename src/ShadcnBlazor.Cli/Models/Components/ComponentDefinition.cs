namespace ShadcnBlazor.Cli.Models.Components;

/// <summary>
/// The internal representation of a component in the CLI, containing all the information required for properly resolving
/// its dependencies, as well as the required actions to be taken to fully integrate it to the client project.
/// </summary>
public class ComponentDefinition
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public string[] Dependencies { get; init; } = [];
    public RequiredAction[] RequiredActions { get; init; } = [];
}

