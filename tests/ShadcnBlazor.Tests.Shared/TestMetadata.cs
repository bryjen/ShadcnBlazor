using System;
using System.Collections.Generic;

namespace ShadcnBlazor.Tests.Shared;

/// <summary>
/// Metadata registry for test components. Maps test type names to descriptive information.
/// </summary>
public static class TestMetadata
{
    public record TestInfo(string Label, string Description);

    private static readonly Dictionary<string, TestInfo> Metadata = new(StringComparer.Ordinal)
    {
        // Button tests
        { "ButtonTest1", new("Variants & States", "Default, Destructive, Outline, Ghost, Link variants; Small and Large sizes; Loading and Disabled states") },
        { "ButtonTest2", new("Callbacks & Types", "OnClick callback, Size.Md default, Variant.Secondary, ButtonType.Button and ButtonType.Reset") },
        { "ButtonTest3", new("ButtonGroup", "ButtonGroup with all 6 variants; Role override to 'toolbar'") },

        // Badge tests
        { "BadgeTest1", new("Variants & Sizes", "Default, Secondary, Destructive, Outline variants; Small and Large sizes") },
        { "BadgeTest2", new("BadgeLink Navigation", "BadgeLink with default href navigation") },
        { "BadgeTest3", new("BadgeLink Variants", "Badge Variant.Link; BadgeLink with all variants and all sizes") },

        // Card tests
        { "CardTest1", new("Header & Footer", "Card with Header and Footer RenderFragments") },
        { "CardTest2", new("Custom Styling", "Card with custom header/footer styling and complex content layout") },
        { "CardTest3", new("Layout Paths", "Content-only (flat path), Header+Content (no Footer), Content+Footer (no Header)") },

        // Checkbox tests
        { "CheckboxTest1", new("Basic States", "Default, Disabled, and Checked states; with and without labels") },
        { "CheckboxTest2", new("Binding & Alignment", "Two-way binding (@bind-Checked) with synced checkboxes; Top, Center, Bottom alignments") },
        { "CheckboxTest3", new("Sizes & Callbacks", "All sizes (Sm, Md, Lg); All alignments with multi-line labels; CheckedChanged callback") },
        { "CheckboxTest4", new("Disabled Guard", "Disabled checkbox guard — proves CheckedChanged does NOT fire when disabled") },

        // Skeleton tests
        { "SkeletonTest1", new("Predefined Shapes", "Skeleton with predefined Class shapes (circle, rectangular, text-line)") },
        { "SkeletonTest2", new("Natural Dimensions", "Bare skeleton (no Class, natural dimensions); minimal size Class variant") },

        // Alert tests
        { "AlertTest1", new("Variants", "Default and Destructive variants") },

        // Combobox tests
        { "ComboboxTest1", new("Search Basics", "Basic combobox search interaction and selected value display") },
        { "ComboboxTest2", new("Grouped Search", "Grouped combobox items, disabled options, and empty-state search path") },

        // MultiSelect tests
        { "MultiSelectTest1", new("Grouped Multi-Select", "Grouped items with search, clear action, and live selected-values summary") },
        { "MultiSelectTest2", new("Sizes & Limits", "Small and large multi-select triggers with max displayed value summaries") },

        // DropdownMenu tests
        { "DropdownMenuTest1", new("Basic Actions", "Menu trigger, clickable items, disabled item, and shortcut labels") },
        { "DropdownMenuTest2", new("Checkboxes & Radios", "Checkbox items and radio-group selection with live state display") },
        { "DropdownMenuTest3", new("Nested Submenus", "Submenu composition for hierarchical command flows") },

        // ContextMenu tests
        { "ContextMenuTest1", new("Basic Context Actions", "Right-click target with rename, duplicate, and destructive delete actions") },
        { "ContextMenuTest2", new("Checkboxes & Radios", "Context menu checkbox items, radio group, and disabled action state") },
        { "ContextMenuTest3", new("Nested Submenus", "Context menu submenu composition for move-style actions") },

        // DataTable tests
        { "DataTableTest1", new("Basic Sortable Table", "Field-based columns with hover state and sortable headers") },
        { "DataTableTest2", new("Multi-Selection", "Toolbar content, striped rows, bound selected items, and clear action") },
        { "DataTableTest3", new("Grid Mode & Custom Cells", "Grid interaction mode, row-click callback, and badge-based custom cells") },

        // Form tests
        { "FormTest1", new("Validation Context", "Input and Button inheriting invalid-state accessibility attributes from FormValidationProvider") },
        { "FormTest2", new("Valid vs Invalid", "Side-by-side comparison of valid and invalid form contexts") },
    };

    public static string? GetLabel(string testName)
    {
        return Metadata.TryGetValue(testName, out var info) ? info.Label : null;
    }

    public static string? GetDescription(string testName)
    {
        return Metadata.TryGetValue(testName, out var info) ? info.Description : null;
    }
}
