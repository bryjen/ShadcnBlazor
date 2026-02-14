namespace ShadcnBlazor.Docs.Services;

/// <summary>
/// Registry of sample applications showcased in the docs.
/// </summary>
public class SampleRegistryService
{
    private static readonly SampleRegistryEntry[] Samples =
    [
        new("AI Chat", "ai-chat", "An interactive chat interface demonstrating AI-style message streaming and structured responses."),
    ];

    public IReadOnlyList<SampleRegistryEntry> SamplesList => Samples;
}

public record SampleRegistryEntry(string Name, string Slug, string Description);
