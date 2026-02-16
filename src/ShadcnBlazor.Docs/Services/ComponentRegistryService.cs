namespace ShadcnBlazor.Docs.Services;

/// <summary>
/// Metadata for a component (Name, Description). Replaces ComponentMetadataAttribute for docs.
/// </summary>
public record ComponentMetadata(string Name, string Description);

/// <summary>
/// Tracks components from the ShadcnBlazor library for docs navigation.
/// Mirrors the CLI's component registry.
/// </summary>
public class ComponentRegistryService
{
    private static readonly IReadOnlyDictionary<string, ComponentMetadata> MetadataByName = new Dictionary<string, ComponentMetadata>(StringComparer.OrdinalIgnoreCase)
    {
        ["Accordion"] = new("Accordion", "A vertically stacked set of interactive headings that each reveal a section of content."),
        ["Alert"] = new("Alert", "Displays important messages or notifications with variant styling (default, destructive)."),
        ["Avatar"] = new("Avatar", "Displays image avatars with text fallback for missing or loading images."),
        ["Badge"] = new("Badge", "Small label or count indicator with variant styling (default, secondary, outline, destructive)."),
        ["Button"] = new("Button", "Clickable button with variants (default, destructive, outline, secondary, ghost, link) and sizes."),
        ["Card"] = new("Card", "Container for content with header, body, and footer sections."),
        ["Checkbox"] = new("Checkbox", "Checkbox input for boolean or multi-select form values."),
        ["Dialog"] = new("Dialog", "Imperative dialog service; show dialogs via IDialogService.Show. Requires DialogProvider in layout."),
        ["DropdownMenu"] = new("DropdownMenu", "Dropdown menu with trigger and content; requires PopoverProvider in layout."),
        ["Input"] = new("Input", "Single-line text input with variant styling."),
        ["Popover"] = new("Popover", "Floating panel anchored to a trigger element; requires PopoverProvider in layout."),
        ["Radio"] = new("Radio", "Radio and RadioCard options for single selection within a RadioGroup."),
        ["Select"] = new("Select", "Dropdown select for choosing a single value from a list of options; requires PopoverProvider in layout."),
        ["Skeleton"] = new("Skeleton", "Loading placeholder with pulse animation."),
        ["Slider"] = new("Slider", "Single-thumb slider for selecting a value within a min/max range."),
        ["Switch"] = new("Switch", "Toggle switch for boolean on/off values."),
        ["Textarea"] = new("Textarea", "Multi-line text input for longer form content."),
        ["Tooltip"] = new("Tooltip", "Hover-triggered tooltip with pointer; requires PopoverProvider in layout."),
    };

    private static readonly IReadOnlyList<ComponentRegistryEntry> AllComponents =
    [
        new ComponentRegistryEntry("Accordion", "accordion"),
        new ComponentRegistryEntry("Alert", "alert"),
        new ComponentRegistryEntry("Avatar", "avatar"),
        new ComponentRegistryEntry("Badge", "badge"),
        new ComponentRegistryEntry("Button", "button"),
        new ComponentRegistryEntry("Card", "card"),
        new ComponentRegistryEntry("Checkbox", "checkbox"),
        new ComponentRegistryEntry("Dialog", "dialog"),
        new ComponentRegistryEntry("DropdownMenu", "dropdown-menu"),
        new ComponentRegistryEntry("Input", "input"),
        new ComponentRegistryEntry("Popover", "popover"),
        new ComponentRegistryEntry("Radio", "radio"),
        new ComponentRegistryEntry("Select", "select"),
        new ComponentRegistryEntry("Skeleton", "skeleton"),
        new ComponentRegistryEntry("Slider", "slider"),
        new ComponentRegistryEntry("Switch", "switch"),
        new ComponentRegistryEntry("Textarea", "textarea"),
        new ComponentRegistryEntry("Tooltip", "tooltip"),
    ];

    private static readonly IReadOnlyList<ComponentRegistryEntry> PseudoComponents =
    [
        new ComponentRegistryEntry("Icons", "icons"),
        new ComponentRegistryEntry("Typography", "typography"),
    ];

    private static readonly HashSet<string> ComponentsWithoutOwnPage = ["ComposableTextArea"];

    /// <summary>
    /// Gets metadata for a component by name.
    /// </summary>
    public static ComponentMetadata GetMetadata(string componentName)
    {
        if (MetadataByName.TryGetValue(componentName, out var meta))
            return meta;
        return new ComponentMetadata(componentName, "");
    }

    /// <summary>
    /// All components, ordered by name.
    /// </summary>
    public IReadOnlyList<ComponentRegistryEntry> Components { get; } =
        AllComponents
            .Where(c => !ComponentsWithoutOwnPage.Contains(c.Name))
            .Concat(PseudoComponents)
            .OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
}

public record ComponentRegistryEntry(string Name, string Slug);
