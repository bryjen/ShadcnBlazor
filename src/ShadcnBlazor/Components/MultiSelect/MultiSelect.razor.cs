using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using ShadcnBlazor.Components.Select;
using ShadcnBlazor.Components.Select.Base;
using ShadcnBlazor.Components.Shared.Models.Enums;

namespace ShadcnBlazor.Components.MultiSelect;

/// <summary>
/// Dropdown select component for choosing multiple values from a list of options.
/// </summary>
public partial class MultiSelect<T>
{
    private HashSet<T> _valuesSet = [];
    private string _searchText = string.Empty;
    private bool _prevOpen;
    private ElementReference _triggerRef;
    private ElementReference _searchInputRef;
    private readonly string _searchInputId = $"select-search-{Guid.NewGuid():N}";

#region Parameters
    /// <summary>Currently selected values.</summary>
    [Parameter]
    public IReadOnlyList<T> Values { get; set; } = [];

    /// <summary>Callback invoked when the selected values change.</summary>
    [Parameter]
    public EventCallback<IReadOnlyList<T>> ValuesChanged { get; set; }

    /// <summary>Optional label displayed at the top of the dropdown content and used as the trigger aria-label.</summary>
    [Parameter]
    public string? Label { get; set; }

    /// <summary>Placeholder text shown when no values are selected.</summary>
    [Parameter]
    public string Placeholder { get; set; } = "Select...";

    /// <summary>Size variant applied to the trigger.</summary>
    [Parameter]
    public Size Size { get; set; } = Size.Md;

    /// <summary>When true, the popover expands to fit the width of its option content instead of matching the trigger width.</summary>
    [Parameter]
    public bool PopoverFitContent { get; set; }

    /// <summary>When true, body scroll is locked while the popover is open.</summary>
    [Parameter]
    public bool LockScroll { get; set; } = true;

    /// <summary>Additional CSS classes applied to the trigger element.</summary>
    [Parameter]
    public string TriggerClass { get; set; } = string.Empty;

    /// <summary>Maximum number of values to display before showing "+ X more".</summary>
    [Parameter]
    public int MaxDisplayedValues { get; set; } = 3;

    /// <summary>When true, shows a search input at the top of the dropdown that focuses on open and keeps focus during keyboard nav.</summary>
    [Parameter]
    public bool Search { get; set; }

    /// <summary>When true, appends a Clear action node at the bottom of the list that resets all selected values.</summary>
    [Parameter]
    public bool ClearButton { get; set; }
#endregion

    protected override void OnParametersSet()
    {
        _valuesSet = [.. Values.Select(v => v)];
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (Search && _open && !_prevOpen)
        {
            try { await _searchInputRef.FocusAsync(); } catch { /* element may not be mounted yet */ }
        }

        _prevOpen = _open;
    }

#region Node overrides
    protected override List<SelectDeclarativeNode> GetRenderNodes()
    {
        var all = base.GetRenderNodes();

        IEnumerable<SelectDeclarativeNode> visible = all;

        if (Search && !string.IsNullOrWhiteSpace(_searchText))
        {
            var search = _searchText.Trim();
            visible = all.Where(n => n.Kind == SelectNodeKind.Item &&
                                     n.Text.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        var result = visible.ToList();

        if (ClearButton)
        {
            result.Add(new SelectDeclarativeNode
            {
                Kind = SelectNodeKind.Action,
                Text = "Clear",
                Callback = HandleClear,
            });
        }

        return result;
    }

    protected override bool IsSelected(SelectDeclarativeNode node)
    {
        if (node.Kind != SelectNodeKind.Item)
            return false;

        if (!TryGetNodeValue(node, out var nodeValue))
            return false;

        if (nodeValue is null)
            return _valuesSet.Count == 0 || _valuesSet.Contains(default!);

        return _valuesSet.Contains(nodeValue);
    }
#endregion

#region Open / close
    protected override Task HandleOpenChanged(bool open)
    {
        if (!open)
            _searchText = string.Empty;

        return base.HandleOpenChanged(open);
    }

    protected override async Task HandleTabKeyAsync(bool shiftKey)
    {
        _open = false;
        await InvokeAsync(StateHasChanged);
        await _triggerRef.FocusAsync();
    }
#endregion

#region Search input
    protected async Task HandleSearchInput(ChangeEventArgs e)
    {
        _searchText = e.Value?.ToString() ?? string.Empty;
        EnsureActiveOnOpen();
        await InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Routes keyboard events from the search input. Printable characters pass through
    /// to @oninput; all navigation keys are forwarded to the base handler so arrow nav,
    /// Enter, and Escape work while focus stays on the input.
    /// </summary>
    protected async Task HandleSearchKeyDown(KeyboardEventArgs e)
    {
        if (e.Key.Length == 1 && !char.IsControl(e.Key[0]))
            return;

        await HandleTriggerKeyDown(e);
    }
#endregion

#region Selection
    protected override async Task OnItemSelectAsync(int idx, T? value)
    {
        // Toggle — do NOT call base (which would close the dropdown).
        SetActiveIndex(idx);

        if (value is null)
        {
            var hasNull = Values.Any(v => v is null);
            var newList = hasNull
                ? Values.Where(v => v is not null).ToList()
                : [.. Values, value!];
            _valuesSet = [.. newList.Where(v => v is not null).Select(v => v!)];
            await ValuesChanged.InvokeAsync(newList);
            return;
        }

        List<T> updated;
        if (_valuesSet.Contains(value))
            updated = Values.Where(v => !EqualityComparer<T>.Default.Equals(v, value)).ToList();
        else
            updated = [.. Values, value];

        _valuesSet = [.. updated.Where(v => v is not null).Select(v => v!)];
        await ValuesChanged.InvokeAsync(updated);
    }

    private async Task HandleClear()
    {
        _valuesSet = [];
        await ValuesChanged.InvokeAsync([]);
    }
#endregion

#region Styling
    private string GetDisplayText()
    {
        if (Values.Count == 0)
            return Placeholder;

        // Always use unfiltered nodes so the trigger label isn't affected by search text.
        var nodes = base.GetRenderNodes();
        var selectedTexts = nodes
            .Where(IsSelected)
            .Select(n => n.Text ?? string.Empty)
            .Where(t => !string.IsNullOrEmpty(t))
            .ToList();

        if (selectedTexts.Count == 0)
            return Placeholder;

        if (selectedTexts.Count <= MaxDisplayedValues)
            return string.Join(", ", selectedTexts);

        var shown = selectedTexts.Take(MaxDisplayedValues);
        var remaining = selectedTexts.Count - MaxDisplayedValues;
        return $"{string.Join(", ", shown)} +{remaining} more";
    }

    private string GetAriaLabel()
    {
        if (!string.IsNullOrWhiteSpace(Label))
            return Label;
        return Placeholder;
    }

    private string GetContentClass() =>
        PopoverFitContent ? "w-max" : "w-[var(--popover-width)]";

    private string GetTriggerClass()
    {
        var baseClasses = string.Join(" ", [
            "flex w-full items-center justify-between gap-2 rounded-md border border-input",
            "bg-input/30 shadow-xs hover:bg-input/50",
            "transition-all duration-200 outline-none focus-visible:border-ring focus-visible:ring-ring/50 focus-visible:ring-[3px] disabled:cursor-not-allowed disabled:opacity-50",
            "[&>svg]:size-4 [&>svg]:shrink-0",
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
        var selectedClasses = isSelected ? "bg-primary/10" : string.Empty;
        var activeClasses = isActive ? "duration-0 bg-primary/30" : string.Empty;
        var disabledClasses = isDisabled ? "cursor-not-allowed opacity-50" : string.Empty;
        var baseClasses = string.Join(" ", [
            "flex w-full items-center gap-2 rounded-md px-2 py-1.5 text-sm",
            "outline-none focus-visible:outline-none transition-colors",
        ]);
        var interactiveClasses = isDisabled ? string.Empty : "cursor-pointer";
        return MergeCss(selectedClasses, activeClasses, disabledClasses, interactiveClasses, baseClasses);
    }

    private string GetActionClass(bool isActive) =>
        MergeCss(
            isActive ? "duration-0 bg-accent" : string.Empty,
            "flex w-full items-center gap-2 rounded-md px-2 py-1.5 text-sm text-muted-foreground",
            "cursor-pointer outline-none transition-colors hover:bg-accent hover:text-accent-foreground"
        );
#endregion
}
