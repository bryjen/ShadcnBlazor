using ShadcnBlazor.Components.Accordion;
using ShadcnBlazor.Components.Alert;
using ShadcnBlazor.Components.Avatar;
using ShadcnBlazor.Components.Badge;
using ShadcnBlazor.Components.Button;
using ShadcnBlazor.Components.Card;
using ShadcnBlazor.Components.Checkbox;
using ShadcnBlazor.Components.DataTable;
using ShadcnBlazor.Components.Dialog.Services;
using ShadcnBlazor.Components.DropdownMenu;
using ShadcnBlazor.Components.Field;
using ShadcnBlazor.Components.Input;
using ShadcnBlazor.Components.Label;
using ShadcnBlazor.Components.Popover;
using ShadcnBlazor.Components.Popover.Services;
using ShadcnBlazor.Components.Shared.Services;
using ShadcnBlazor.Components.Shared.Services.Interop;
using ShadcnBlazor.Components.Skeleton;
using ShadcnBlazor.Components.Switch;
using ShadcnBlazor.Components.Textarea;
using ShadcnBlazor.Components.ToggleButton;
using ShadcnBlazor.Components.Tooltip;
using ShadcnBlazor.Components.FocusTrap;
using ShadcnBlazor.Components.FocusTrap.Services;
using ShadcnBlazor.Components.Sonner.Services;
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

    /// <summary>
    /// Gets the list of all registered components in the library.
    /// </summary>
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
                new CopyJsAction("key-interceptor.js"),
                new AddCssLinksToRootAction(),
                new AddNugetDependencyAction("TailwindMerge.NET", "1.2.0"),
                new AddProgramServiceAction("TailwindMerge.Extensions", "AddTailwindMerge()"),
                new AddToServicesAction(nameof(KeyInterceptorInterop)),
                new MergeToImportsAction([
                    "ShadcnBlazor.Components.Shared",
                    "ShadcnBlazor.Components.Shared.Models",
                    "ShadcnBlazor.Components.Shared.Models.Enums",
                    "ShadcnBlazor.Components.Shared.Models.Options",
                ]),
            ]
        },
        new()
        {
            Name = "Combobox",
            Description = "Searchable dropdown that lets the user filter and pick a single value; requires PopoverProvider in layout.",
            Dependencies = CreateDeps(nameof(Popover)),
            Tags = [ComponentDefinition.Tag.Alpha]
        },
        new()
        {
            Name = nameof(Accordion),
            Description = "A vertically stacked set of interactive headings that each reveal a section of content.", 
            Dependencies = CreateDeps(),
        },
        new() 
        {
            Name = nameof(Alert), 
            Description = "Displays important messages or notifications with variant styling (default, destructive).", 
            Dependencies = CreateDeps()
        },
        new() 
        {
            Name = nameof(Avatar), 
            Description = "Displays image avatars with text fallback for missing or loading images.", 
            Dependencies = CreateDeps()
        },
        new() 
        {
            Name = nameof(Badge), 
            Description = "Small label or count indicator with variant styling (default, secondary, outline, destructive).", 
            Dependencies = CreateDeps()
        },
        new()
        {
            Name = nameof(Button), 
            Description = "Clickable button with variants (default, destructive, outline, secondary, ghost, link) and sizes.", 
            Dependencies = CreateDeps(),
        },
        new() 
        {
            Name = nameof(ToggleButton), 
            Description = "Button that toggles between two visual states (on/off). Depends on Button.", Dependencies = CreateDeps(nameof(Button))
        },
        new() 
        {
            Name = nameof(Card), 
            Description = "Container for content with header, body, and footer sections.", 
            Dependencies = CreateDeps(),
        },
        new()
        {
            Name = nameof(Field),
            Description = "Compose labels, controls, helper text, and validation into accessible form fields.",
            Dependencies = CreateDeps(nameof(Label))
        },
        new()
        {
            Name = nameof(Label),
            Description = "Accessible label associated with a form control.",
            Dependencies = CreateDeps()
        },
        new() 
        {
            Name = nameof(Checkbox), 
            Description = "Checkbox input for boolean or multi-select form values.", 
            Dependencies = CreateDeps(),
        },
        new()
        {
            Name = "Dialog",
            Description = "Declarative dialog component using DialogRoot, DialogTrigger, DialogContent, and related composable pieces.",
            Dependencies = CreateDeps(),
            RequiredActions =
            [
                new AddToServicesAction(nameof(DialogInterop)),
                new AddToServicesAction(nameof(ScrollLockService)),
                new CopyJsAction("dialog.js"),
                new CopyJsAction("scroll-lock.js"),
                new MergeToImportsAction([
                    "ShadcnBlazor.Components.Dialog",
                    "ShadcnBlazor.Components.Dialog.Services"]),
            ],
            Tags = []
        },
        new()
        {
            Name = "DataTable",
            Description = "Flexible, accessible data table with sorting, pagination, multi-selection, and keyboard navigation.",
            Dependencies = CreateDeps(),
        },
        new()
        {
            Name = nameof(DropdownMenu),
            Description = "Dropdown menu with trigger and content; requires PopoverProvider in layout.",
            Dependencies = CreateDeps(nameof(Popover)),
            Tags = [ComponentDefinition.Tag.WorkRequired]
        },
        new()
        {
            Name = nameof(Input),
            Description = "Single-line text input with variant styling.",
            Dependencies = CreateDeps(),
        },
        new()
        {
            Name = nameof(Popover),
            Description = "Floating panel anchored to a trigger element; requires PopoverProvider in layout.",
            Dependencies = CreateDeps(),
            RequiredActions =
            [
                new AddToServicesAction(nameof(IPopoverRegistry), nameof(PopoverRegistry)),
                new AddToServicesAction(nameof(PopoverInterop)),
                new AddToServicesAction(nameof(IPopoverService), nameof(PopoverService)),
                new CopyJsAction("popovers.js"),
            ]
        },
        new()
        {
            Name = "Radio Group",
            Description = "RadioGroup with RadioItem and RadioCard options for single selection.",
            Dependencies = CreateDeps()
        },
        new()
        {
            Name = "Select",
            Description = "Dropdown select for choosing a single value from a list of options; requires PopoverProvider in layout.", Dependencies = CreateDeps(nameof(Popover))
        },
        new()
        {
            Name = "MultiSelect",
            Description = "Dropdown select for choosing multiple values from a list; requires PopoverProvider in layout.",
            Dependencies = CreateDeps(nameof(Popover)),
            Tags = [ComponentDefinition.Tag.Alpha]
        },
        new() 
        {
            Name = nameof(Skeleton), 
            Description = "Loading placeholder with pulse animation.", 
            Dependencies = CreateDeps(),
        },
        new()
        {
            Name = nameof(Switch), 
            Description = "Toggle switch for boolean on/off values.", 
            Dependencies = CreateDeps()
        },
        new()
        {
            Name = nameof(Textarea),
            Description = "Multi-line text input for longer form content.",
            Dependencies = CreateDeps()
        },
        new()
        {
            Name = "Tabs",
            Description = "A set of layered sections of content—known as tab panels—that are displayed one at a time.",
            Dependencies = CreateDeps(),
        },
        new()
        {
            Name = nameof(Tooltip),
            Description = "Hover-triggered tooltip with pointer; requires PopoverProvider in layout.", Dependencies = CreateDeps(nameof(Popover))
        },
        new()
        {
            Name = "Sonner",
            Description = "An opinionated toast component for Blazor.",
            RequiredActions =
            [
                new AddToServicesAction(nameof(SonnerService)),
                new AddToServicesAction(nameof(SonnerComponentRegistry)),
            ]
        },
        new()
        {
            Name = nameof(FocusTrap),
            Description = "Traps focus within a container using the focus-trap library.",
            Dependencies = CreateDeps(),
            RequiredActions =
            [
                new AddToServicesAction(nameof(FocusJsInterop)),
                new AddToServicesAction(nameof(FocusService)),
                new MergeToImportsAction([
                    "ShadcnBlazor.Components.FocusTrap",
                    "ShadcnBlazor.Components.FocusTrap.Models",
                    "ShadcnBlazor.Components.FocusTrap.Services"
                ]),
            ]
        },
        new()
        {
            Name = "Drawer",
            Description = "Declarative drawer component for Blazor using Vaul.",
            Dependencies = CreateDeps(),
            RequiredActions =
            [
                new AddToServicesAction(nameof(DrawerComponentRegistry)),
                new AddToServicesAction(nameof(ScrollLockService)),
                new CopyJsAction("vaul-interop.iife.js"),
                new MergeToImportsAction([
                    "ShadcnBlazor.Components.Drawer",
                    "ShadcnBlazor.Services"
                ]),
            ],
            Tags = []
        },
    ];
}
