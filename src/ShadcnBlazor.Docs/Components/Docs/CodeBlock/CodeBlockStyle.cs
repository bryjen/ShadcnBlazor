namespace ShadcnBlazor.Docs.Components.Docs.CodeBlock;

/// <summary>
/// Visual style for the CodeBlock component.
/// </summary>
public enum CodeBlockStyle
{
    /// <summary>Standalone: no border, full rounded corners.</summary>
    Solo,

    /// <summary>Embedded in preview: top border from parent context, bottom rounded only.</summary>
    Embedded
}
