namespace SampleBlazorProject.Shared;

/// <summary>
/// Attribute that provides metadata for ShadcnBlazor components, used by the CLI for discovery and dependency resolution.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ComponentMetadataAttribute : Attribute
{
    /// <summary>
    /// The display name of the component.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// A description of the component's purpose and usage.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Names of other components that this component depends on.
    /// </summary>
    public required string[] Dependencies { get; set; }
}
