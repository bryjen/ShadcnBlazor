using ShadcnBlazor.Components.Accordion;
using ShadcnBlazor.Components.Alert;
using ShadcnBlazor.Components.Avatar;
using ShadcnBlazor.Components.Badge;
using ShadcnBlazor.Components.Button;
using ShadcnBlazor.Components.Card;
using ShadcnBlazor.Components.Checkbox;
using ShadcnBlazor.Components.Dialog;
using ShadcnBlazor.Components.Dialog.Services;
using ShadcnBlazor.Components.DropdownMenu;
using ShadcnBlazor.Components.Input;
using ShadcnBlazor.Components.Popover;
using ShadcnBlazor.Components.Popover.Services;
using ShadcnBlazor.Components.Radio;
using ShadcnBlazor.Components.Select;
using ShadcnBlazor.Components.Shared.Services;
using ShadcnBlazor.Components.Skeleton;
using ShadcnBlazor.Components.Slider;
using ShadcnBlazor.Components.Switch;
using ShadcnBlazor.Components.Textarea;
using ShadcnBlazor.Components.ToggleButton;
using ShadcnBlazor.Components.Tooltip;
using ShadcnBlazor.Services.Models;

namespace ShadcnBlazor.Services;

/// <summary>
/// Registry of all components in the ShadcnBlazor library.
/// </summary>
public static class ComponentRegistry
{
    private static class Shared
    {
    }

    private static string[] CreateDeps(params string[] deps) =>
        Array.Empty<string>()
            .Concat(deps)
            .Concat([nameof(Shared)])
            .ToArray();

    public static readonly ComponentDefinition[] AllComponents =
    [
        new()
        {
            Name = nameof(Shared),
            Description = "Base classes, enums, services, and utilities required by all components",
            RequiredActions =
            [
                new CopyCssAction("shadcn_blazor_in.css"),
                new CopyCssAction("shadcn_blazor_out.css"),
                new AddCssLinksToRootAction(),
                new AddNugetDependencyAction("TailwindMerge.NET", "1.2.0"),
                new AddProgramServiceAction("TailwindMerge.Extensions", "AddTailwindMerge()"),
                new MergeToImportsAction([
                    "ShadcnBlazor.Components.Shared",
                    "ShadcnBlazor.Components.Shared.Models",
                    "ShadcnBlazor.Components.Shared.Models.Enums",
                    "ShadcnBlazor.Components.Shared.Models.Options",
                ]),
            ]
        },
        new() { Name = nameof(Accordion), Description = "A vertically stacked set of interactive headings that each reveal a section of content.", Dependencies = CreateDeps() },
        new() { Name = nameof(Alert), Description = "Displays important messages or notifications with variant styling (default, destructive).", Dependencies = CreateDeps() },
        new() { Name = nameof(Avatar), Description = "Displays image avatars with text fallback for missing or loading images.", Dependencies = CreateDeps() },
        new() { Name = nameof(Badge), Description = "Small label or count indicator with variant styling (default, secondary, outline, destructive).", Dependencies = CreateDeps() },
        new() { Name = nameof(Button), Description = "Clickable button with variants (default, destructive, outline, secondary, ghost, link) and sizes.", Dependencies = CreateDeps() },
        new() { Name = nameof(ToggleButton), Description = "Button that toggles between two visual states (on/off). Depends on Button.", Dependencies = CreateDeps(nameof(Button)) },
        new() { Name = nameof(Card), Description = "Container for content with header, body, and footer sections.", Dependencies = CreateDeps() },
        new() { Name = nameof(Checkbox), Description = "Checkbox input for boolean or multi-select form values.", Dependencies = CreateDeps() },
        new()
        {
            Name = nameof(Dialog),
            Description = "Imperative dialog service; show dialogs via IDialogService.Show. Requires DialogProvider in layout.",
            Dependencies = CreateDeps(),
            RequiredActions =
            [
                new AddToServicesAction(nameof(IDialogService), nameof(DialogService)),
                new AddToServicesAction(nameof(IScrollLockService), nameof(ScrollLockService)),
                new CopyJsAction("dialog.js"),
                new CopyJsAction("scroll-lock.js"),
                new CopyJsAction("keyInterceptor.js"),
                new MergeToImportsAction([
                    "ShadcnBlazor.Components.Dialog",
                    "ShadcnBlazor.Components.Dialog.Declarative",
                    "ShadcnBlazor.Components.Dialog.Models"]),
            ]
        },
        new() { Name = nameof(DropdownMenu), Description = "Dropdown menu with trigger and content; requires PopoverProvider in layout.", Dependencies = CreateDeps(nameof(Popover)) },
        new() { Name = nameof(Input), Description = "Single-line text input with variant styling.", Dependencies = CreateDeps() },
        new()
        {
            Name = nameof(Popover),
            Description = "Floating panel anchored to a trigger element; requires PopoverProvider in layout.",
            Dependencies = CreateDeps(),
            RequiredActions =
            [
                new AddToServicesAction(nameof(IPopoverService), nameof(PopoverService)),
                new AddToServicesAction(nameof(IPopoverRegistry), nameof(PopoverRegistry)),
                new CopyJsAction("popovers.js"),
            ]
        },
        new() { Name = nameof(Radio), Description = "Radio and RadioCard options for single selection within a RadioGroup.", Dependencies = CreateDeps() },
        new() { Name = "Select", Description = "Dropdown select for choosing a single value from a list of options; requires PopoverProvider in layout.", Dependencies = CreateDeps(nameof(Popover)) },
        new() { Name = nameof(Skeleton), Description = "Loading placeholder with pulse animation.", Dependencies = CreateDeps() },
        new() { Name = nameof(Slider), Description = "Single-thumb slider for selecting a value within a min/max range.", Dependencies = CreateDeps() },
        new() { Name = nameof(Switch), Description = "Toggle switch for boolean on/off values.", Dependencies = CreateDeps() },
        new() { Name = nameof(Textarea), Description = "Multi-line text input for longer form content.", Dependencies = CreateDeps() },
        new() { Name = nameof(Tooltip), Description = "Hover-triggered tooltip with pointer; requires PopoverProvider in layout.", Dependencies = CreateDeps(nameof(Popover)) },
    ];
}
