using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Select.Base;
using ShadcnBlazor.Components.Shared.Models.Enums;

namespace ShadcnBlazor.Components.Select;

/// <summary>
/// Dropdown select component for choosing a single value from a list of options.
/// </summary>
public partial class Select<T>
{
    /// <summary>
    /// Currently selected value.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Data)]
    public T? Value { get; set; }

    /// <summary>
    /// Callback invoked when the selected value changes.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public EventCallback<T?> ValueChanged { get; set; }

    /// <summary>
    /// Optional label displayed at the top of the dropdown content and used as the trigger aria-label.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Common)]
    public string? Label { get; set; }

    /// <summary>
    /// Placeholder text shown when no value is selected.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Common)]
    public string Placeholder { get; set; } = "Select...";

    /// <summary>
    /// Size variant applied to the trigger.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Appearance)]
    public Size Size { get; set; } = Size.Md;

    /// <summary>
    /// When true, the popover expands to fit the width of its option content instead of matching the trigger width.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public bool PopoverFitContent { get; set; }

    /// <summary>
    /// When true, body scroll is locked while the popover is open.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public bool LockScroll { get; set; } = true;

    /// <summary>
    /// Additional CSS classes applied to the trigger element.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Appearance)]
    public string TriggerClass { get; set; } = string.Empty;

    /// <inheritdoc />
    protected override bool IsSelected(SelectDeclarativeNode node)
    {
        if (node.Kind != SelectNodeKind.Item)
            return false;

        if (!TryGetNodeValue(node, out var nodeValue))
            return false;

        if (nodeValue is null && Value is null)
            return true;

        if (nodeValue is null || Value is null)
            return false;

        return EqualityComparer<T>.Default.Equals(nodeValue, Value);
    }

    /// <inheritdoc />
    protected override async Task OnItemSelectAsync(int idx, T? value)
    {
        await base.OnItemSelectAsync(idx, value);
        Value = value;
        await ValueChanged.InvokeAsync(Value);
    }

    private string GetContentClass()
    {
        return PopoverFitContent
            ? "w-max"
            : "w-[var(--popover-width)]";
    }

    private string GetTriggerClass()
    {
        var baseClasses = string.Join(" ", [
            "flex w-full items-center justify-between gap-2 rounded-md border border-input",
            "bg-transparent shadow-xs",
            "transition-[color,box-shadow] outline-none focus-visible:border-ring focus-visible:ring-ring/50 focus-visible:ring-[3px] disabled:cursor-not-allowed disabled:opacity-50",
            "whitespace-nowrap dark:bg-input/30 dark:hover:bg-input/50",
            "[&_svg]:pointer-events-none [&_svg]:shrink-0 [&_svg]:size-4",
        ]);
        var sizeClasses = Size switch
        {
            Size.Sm => "h-8 px-3 py-2 text-sm",
            Size.Md => "h-9 px-3 py-2 text-sm",
            Size.Lg => "h-10 px-3 py-2 text-base md:text-sm",
            _ => "h-9 px-3 py-2 text-sm",
        };
        return MergeCss(baseClasses, sizeClasses, TriggerClass);
    }

    private string GetOptionClass(bool isSelected, bool isActive, bool isDisabled)
    {
        var selectedClasses = isSelected
            ? "bg-primary/20 ring-1 ring-inset ring-primary"
            : string.Empty;
        var activeClasses = isActive ? "duration-0 bg-primary/10" : string.Empty;
        var disabledClasses = isDisabled ? "cursor-not-allowed opacity-50" : string.Empty;
        var baseClasses = string.Join(" ", [
            "relative flex w-full items-center gap-2 rounded-sm pr-8 pl-2 py-1.5 text-sm",
            "outline-hidden select-none focus:bg-primary/20 focus:ring-1 focus:ring-primary transition-colors",
        ]);
        var interactiveClasses = isDisabled ? string.Empty : "cursor-default";
        return MergeCss(activeClasses, selectedClasses, disabledClasses, interactiveClasses, baseClasses);
    }

    private string GetAriaLabel()
    {
        if (!string.IsNullOrWhiteSpace(Label))
            return Label;

        return Placeholder;
    }

    private string GetDisplayText()
    {
        if (Value is null)
            return Placeholder;

        var nodes = GetRenderNodes();
        var selected = nodes.FirstOrDefault(IsSelected);
        return selected?.Text ?? Placeholder;
    }
}
