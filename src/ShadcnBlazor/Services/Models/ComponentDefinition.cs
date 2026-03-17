namespace ShadcnBlazor.Services.Models;

/// <summary>
/// Defines a component in the ShadcnBlazor library, including metadata and install actions.
/// </summary>
public class ComponentDefinition
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public string[] Dependencies { get; init; } = [];
    public RequiredAction[] RequiredActions { get; init; } = [];
    public Tag[] Tags { get; init; } = [];

    /// <summary>
    /// URL slug derived from Name: lowercase with spaces replaced by dashes.
    /// </summary>
    public string Slug => Name.ToLowerInvariant().Replace(" ", "-");

    public enum Tag
    {
        Alpha,
        Beta
    }
}
