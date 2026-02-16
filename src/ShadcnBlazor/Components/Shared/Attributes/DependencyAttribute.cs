namespace ShadcnBlazor.Components.Shared.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public abstract class DependencyAttribute : Attribute;

[AttributeUsage(AttributeTargets.Class)]
public abstract class NugetDependencyAttribute : DependencyAttribute
{
    public required string Name { get; set; }

    public required string Version { get; set; }
}

[AttributeUsage(AttributeTargets.Class)]
public abstract class ExternalJsDependencyAttribute : DependencyAttribute
{
    public required string Name { get; set; }

    public required string Version { get; set; }
}
