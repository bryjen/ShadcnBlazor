namespace ShadcnBlazor.Docs.Components.Docs.CodeBlock;

public class CodeFile
{
    public string FileName { get; set; } = string.Empty;
    public string Contents { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;

    public static CodeFile Create(string contents, string language, string? fileName = null) =>
        new() { Contents = contents, Language = language, FileName = fileName ?? "" };
}