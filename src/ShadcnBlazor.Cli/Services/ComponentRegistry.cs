using ShadcnBlazor.Cli.Models;

namespace ShadcnBlazor.Cli.Services;

public static class ComponentRegistry
{
    public static readonly ComponentDefinition[] AllComponents =
    [
        new() { Name = "Shared", Description = "Base classes, enums, services, and utilities required by all components", Dependencies = [] },
        new() { Name = "Accordion", Description = "A vertically stacked set of interactive headings that each reveal a section of content.", Dependencies = ["Shared"] },
        new() { Name = "Alert", Description = "Displays important messages or notifications with variant styling (default, destructive).", Dependencies = ["Shared"] },
        new() { Name = "Avatar", Description = "Displays image avatars with text fallback for missing or loading images.", Dependencies = ["Shared"] },
        new() { Name = "Badge", Description = "Small label or count indicator with variant styling (default, secondary, outline, destructive).", Dependencies = ["Shared"] },
        new() { Name = "Button", Description = "Clickable button with variants (default, destructive, outline, secondary, ghost, link) and sizes.", Dependencies = ["Shared"] },
        new() { Name = "Card", Description = "Container for content with header, body, and footer sections.", Dependencies = ["Shared"] },
        new() { Name = "Checkbox", Description = "Checkbox input for boolean or multi-select form values.", Dependencies = ["Shared"] },
        new() { Name = "ComposableTextArea", Description = "Multi-line text input with optional header and footer slots.", Dependencies = ["Shared"] },
        new() { Name = "Dialog", Description = "Imperative dialog service; show dialogs via IDialogService.Show. Requires DialogProvider in layout.", Dependencies = ["Shared"] },
        new() { Name = "DropdownMenu", Description = "Dropdown menu with trigger and content; requires PopoverProvider in layout.", Dependencies = ["Shared", "Popover"] },
        new() { Name = "Input", Description = "Single-line text input with variant styling.", Dependencies = ["Shared"] },
        new() { Name = "Popover", Description = "Floating panel anchored to a trigger element; requires PopoverProvider in layout.", Dependencies = ["Shared"] },
        new() { Name = "Radio", Description = "Radio and RadioCard options for single selection within a RadioGroup.", Dependencies = ["Shared"] },
        new() { Name = "Select", Description = "Dropdown select for choosing a single value from a list of options; requires PopoverProvider in layout.", Dependencies = ["Shared", "Popover"] },
        new() { Name = "Skeleton", Description = "Loading placeholder with pulse animation.", Dependencies = ["Shared"] },
        new() { Name = "Slider", Description = "Single-thumb slider for selecting a value within a min/max range.", Dependencies = ["Shared"] },
        new() { Name = "Switch", Description = "Toggle switch for boolean on/off values.", Dependencies = ["Shared"] },
        new() { Name = "Textarea", Description = "Multi-line text input for longer form content.", Dependencies = ["Shared"] },
        new() { Name = "Tooltip", Description = "Hover-triggered tooltip with pointer; requires PopoverProvider in layout.", Dependencies = ["Shared", "Popover"] },
    ];
}
