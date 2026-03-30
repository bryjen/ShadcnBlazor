namespace ShadcnBlazor.Services.Models;

/// <summary>
/// Defines a component in the ShadcnBlazor library, including metadata and install actions.
/// </summary>
public class ComponentDefinition
{
    /// <summary>
    /// Gets the name of the component.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the description of the component.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Gets the list of dependent components.
    /// </summary>
    public string[] Dependencies { get; init; } = [];

    /// <summary>
    /// Gets the list of actions required to install the component.
    /// </summary>
    public RequiredAction[] RequiredActions { get; init; } = [];

    /// <summary>
    /// Gets transparency or status tags for the component.
    /// </summary>
    public Tag[] Tags { get; init; } = [];

    /// <summary>
    /// URL slug derived from Name: lowercase with spaces replaced by dashes.
    /// </summary>
    public string Slug => Name.ToLowerInvariant().Replace(" ", "-");

    /// <summary>
    /// Status tags for development state.
    /// </summary>
    public enum Tag
    {
        /// <summary>Component is still being built and lacks full functionality.</summary>
        WorkRequired,
        /// <summary>Component is in early testing and may have breaking changes.</summary>
        Alpha,
        /// <summary>Component is feature-complete but undergoing stabilization.</summary>
        Beta
    }
}
