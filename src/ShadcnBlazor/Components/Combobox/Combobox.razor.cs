using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ShadcnBlazor.Components.Select;
using ShadcnBlazor.Components.Select.Base;
using ShadcnBlazor.Components.Shared.Models.Enums;

namespace ShadcnBlazor.Components.Combobox;

/// <summary>
/// Searchable dropdown that lets the user filter and pick a single value.
/// </summary>
public partial class Combobox<T> : SelectBase<T>
{
    private string _searchText = string.Empty;

#region Parameters
    /// <summary>Currently selected value.</summary>
    [Parameter]
    public T? Value { get; set; }

    /// <summary>Callback invoked when the selected value changes.</summary>
    [Parameter]
    public EventCallback<T?> ValueChanged { get; set; }

    /// <summary>Optional label shown at the top of the dropdown and used as the trigger aria-label.</summary>
    [Parameter]
    public string? Label { get; set; }

    /// <summary>Placeholder text shown when no value is selected.</summary>
    [Parameter]
    public string Placeholder { get; set; } = "Search...";

    /// <summary>Size variant applied to the trigger input.</summary>
    [Parameter]
    public Size Size { get; set; } = Size.Md;

    /// <summary>When true, the popover expands to fit option content instead of matching trigger width.</summary>
    [Parameter]
    public bool PopoverFitContent { get; set; }

    /// <summary>When true, body scroll is locked while the popover is open.</summary>
    [Parameter]
    public bool LockScroll { get; set; } = true;

    /// <summary>Additional CSS classes applied to the trigger input element.</summary>
    [Parameter]
    public string TriggerClass { get; set; } = string.Empty;
#endregion

#region Overrides
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

    /// <summary>
    /// Returns nodes filtered by the current search text.
    /// Non-item nodes (labels, separators) are dropped when there is an active search
    /// so the flattened results are clean.
    /// </summary>
    protected override List<SelectDeclarativeNode> GetRenderNodes()
    {
        var all = base.GetRenderNodes();

        if (string.IsNullOrWhiteSpace(_searchText))
            return all;

        var search = _searchText.Trim();
        return all
            .Where(n => n.Kind == SelectNodeKind.Item &&
                        n.Text.Contains(search, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    protected override void EnsureActiveOnOpen()
    {
        // Always point to the first match so the user sees what Enter will select.
        var nodes = GetRenderNodes();
        if (nodes.Count == 0)
        {
            SetActiveIndex(null);
            return;
        }

        // Prefer the currently selected item when no search text.
        if (string.IsNullOrWhiteSpace(_searchText))
        {
            var selectedIdx = nodes
                .Select((n, i) => (n, i))
                .FirstOrDefault(x => IsSelected(x.n) && !x.n.Disabled);

            if (selectedIdx.n is not null)
            {
                SetActiveIndex(selectedIdx.i);
                return;
            }
        }

        // Otherwise first selectable
        for (var i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].Kind == SelectNodeKind.Item && !nodes[i].Disabled)
            {
                SetActiveIndex(i);
                return;
            }
        }

        SetActiveIndex(null);
    }

    protected override Task HandleOpenChanged(bool open)
    {
        if (open)
            _searchText = string.Empty;

        return base.HandleOpenChanged(open);
    }

    protected override async Task OnItemSelectAsync(int idx, T? value)
    {
        // idx is relative to the current filtered list — grab text before closing.
        var nodes = GetRenderNodes();
        _searchText = idx < nodes.Count ? nodes[idx].Text : string.Empty;

        await base.OnItemSelectAsync(idx, value);
        Value = value;
        await ValueChanged.InvokeAsync(Value);
    }

    /// <summary>
    /// Tab on a combobox input closes the dropdown and lets the browser move focus naturally.
    /// We do NOT select the active item — the user must explicitly press Enter.
    /// </summary>
    protected override async Task HandleTabKeyAsync(bool shiftKey)
    {
        if (_open)
        {
            _open = false;
            await InvokeAsync(StateHasChanged);
        }
        // Let the browser handle focus movement — no JS needed for a real input element.
    }
#endregion

#region Search
    protected async Task HandleSearchInput(ChangeEventArgs e)
    {
        _searchText = e.Value?.ToString() ?? string.Empty;

        if (!_open)
        {
            _open = true;
        }

        // After filtering, always point active to the first visible match.
        EnsureActiveOnOpen();
        await InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Routes keyboard events from the input. Printable characters are left to the browser
    /// so they appear in the input; only navigation keys are forwarded to the base handler.
    /// </summary>
    protected async Task HandleComboboxKeyDown(KeyboardEventArgs e)
    {
        // Single printable character → let @oninput handle it, don't interfere.
        if (e.Key.Length == 1 && !char.IsControl(e.Key[0]))
            return;

        await HandleTriggerKeyDown(e);
    }
#endregion

#region Styling
    private string GetDisplayValue() => _searchText;

    private string GetAriaLabel()
    {
        if (!string.IsNullOrWhiteSpace(Label))
            return Label;
        return Placeholder;
    }

    private string GetContentClass() => PopoverFitContent ? "w-max" : "w-[var(--popover-width)]";

    private string GetTriggerClass()
    {
        var baseClasses = string.Join(" ", [
            "flex w-full items-center gap-2 rounded-md border border-input",
            "bg-input/30 shadow-xs hover:bg-input/50",
            "transition-all duration-200 outline-none focus-visible:border-ring focus-visible:ring-ring/50 focus-visible:ring-[3px] disabled:cursor-not-allowed disabled:opacity-50",
        ]);
        var sizeClasses = Size switch
        {
            Size.Sm => "h-6 px-1.75 py-0.75 text-[0.6rem]",
            Size.Md => "h-7 px-2.5 py-1 text-sm",
            Size.Lg => "h-8 px-2.75 py-1.25 text-base md:text-sm",
            _ => "h-7 px-2.5 py-1 text-sm",
        };
        return MergeCss(baseClasses, sizeClasses, TriggerClass);
    }

    private string GetOptionClass(bool isSelected, bool isActive, bool isDisabled)
    {
        var selectedClasses = isSelected ? "bg-primary text-primary-foreground" : string.Empty;
        var activeClasses = isActive ? "duration-0 bg-primary/30" : string.Empty;
        var disabledClasses = isDisabled ? "cursor-not-allowed opacity-50" : string.Empty;
        var baseClasses = string.Join(" ", [
            "flex w-full items-center gap-2 rounded-md px-2 py-1.5 text-sm",
            "outline-none focus-visible:outline-none transition-colors",
        ]);
        var interactiveClasses = isDisabled ? string.Empty : "cursor-pointer";
        return MergeCss(activeClasses, selectedClasses, disabledClasses, interactiveClasses, baseClasses);
    }
#endregion
}
