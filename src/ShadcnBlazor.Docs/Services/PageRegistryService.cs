namespace ShadcnBlazor.Docs.Services;

/// <summary>
/// Registry of standalone documentation pages (sections, index pages).
/// </summary>
public class PageRegistryService
{
    private static readonly PageRegistryEntry[] Pages =
    [
        new("Home", "", "/"),
        new("Introduction", "introduction", "/introduction"),
        new("Components", "components", "/components"),
        new("Samples", "samples", "/samples"),
        new("CLI", "cli", "/cli"),
    ];

    public IReadOnlyList<PageRegistryEntry> PagesList => Pages;
}

public record PageRegistryEntry(string Name, string Slug, string Href);
