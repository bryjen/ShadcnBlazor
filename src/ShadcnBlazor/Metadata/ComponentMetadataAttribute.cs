namespace ShadcnBlazor.Metadata;

[AttributeUsage(AttributeTargets.Class)]
public class ComponentMetadataAttribute : Attribute
{
    public required string Name { get; set; }
    public required string[] Dependencies { get; set; }
    public required string[] Description { get; set; }
}