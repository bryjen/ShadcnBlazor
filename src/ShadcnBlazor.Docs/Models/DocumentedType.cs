namespace ShadcnBlazor.Docs.Models;

public class DocumentedType
{
    public string Name { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
    public DocumentedProperty[] Properties { get; init; } = Array.Empty<DocumentedProperty>();
    public DocumentedMethod[] Methods { get; init; } = Array.Empty<DocumentedMethod>();
    public DocumentedEvent[] Events { get; init; } = Array.Empty<DocumentedEvent>();
}

public class DocumentedProperty
{
    public string Name { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
    public string Remarks { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string DefaultValue { get; init; } = string.Empty;
}

public class DocumentedMethod
{
    public string Name { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
    public string ReturnType { get; init; } = string.Empty;
    public string Returns { get; init; } = string.Empty;
}

public class DocumentedEvent
{
    public string Name { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
}
