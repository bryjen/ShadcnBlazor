using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using ShadcnBlazor.Components.Shared.Models.Enums;

namespace ShadcnBlazor.Components.Select;

/// <summary>
/// Dropdown select component for choosing a single value from a list of options.
/// </summary>
public partial class Select<T>
{
    /// <summary>
    /// Optional label displayed at the top of the dropdown content and used as the trigger aria-label.
    /// </summary>
    [Parameter]
    public string? Label { get; set; }

    /// <summary>
    /// Currently selected value.
    /// </summary>
    [Parameter]
    public T? Value { get; set; }

    /// <summary>
    /// Callback invoked when the selected value changes.
    /// </summary>
    [Parameter]
    public EventCallback<T?> ValueChanged { get; set; }

    /// <summary>
    /// Programmatic list of options to render. Do not use together with declarative child items.
    /// </summary>
    [Parameter]
    public IEnumerable<SelectOption<T>> Items { get; set; } = [];

    /// <summary>
    /// Declarative option content (for example SelectItem, SelectLabel, SelectSeparator, SelectGroup).
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Placeholder text shown when no value is selected.
    /// </summary>
    [Parameter]
    public string Placeholder { get; set; } = "Select...";

    /// <summary>
    /// Size variant applied to the trigger.
    /// </summary>
    [Parameter]
    public Size Size { get; set; } = Size.Md;

    /// <summary>
    /// When true, the popover expands to fit the width of its option content instead of matching the trigger width.
    /// </summary>
    [Parameter]
    public bool PopoverFitContent { get; set; }

    /// <summary>
    /// When true, body scroll is locked while the popover is open.
    /// </summary>
    [Parameter]
    public bool LockScroll { get; set; } = true;

    /// <summary>
    /// Disables the trigger and prevents interaction.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Additional CSS classes applied to the trigger element.
    /// </summary>
    [Parameter]
    public string TriggerClass { get; set; } = string.Empty;

    /// <summary>
    /// Maximum number of visible options before the list becomes scrollable. When null or less than 1, no item-count cap is applied.
    /// </summary>
    [Parameter]
    public int? MaxVisibleItems { get; set; }

    /// <summary>
    /// Custom template used to render each selectable item.
    /// </summary>
    [Parameter]
    public RenderFragment<SelectOption<T>>? RenderItem { get; set; }

    private readonly SelectDeclarativeRegistry _declarativeRegistry = new();
    private readonly string _triggerId = $"select-trigger-{Guid.NewGuid():N}";
    private readonly string _listboxId = $"select-listbox-{Guid.NewGuid():N}";

    private bool _open;
    private int? _activeIndex;
    private string? _pendingScrollOptionId;
    private IJSObjectReference? _module;
    private bool _maxVisibleItemsListenerRegistered;
    private int _lastMeasuredNodeCount = -1;
    private int? _lastMeasuredMaxVisibleItems;
    private const int PageJumpSize = 10;

    protected override void OnInitialized()
    {
        _declarativeRegistry.Changed += HandleDeclarativeChanged;
    }

    private void HandleDeclarativeChanged()
    {
        _ = InvokeAsync(StateHasChanged);
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
        var selectedClasses = isSelected
            ? "bg-primary text-primary-foreground"
            : string.Empty;
        var activeClasses = isActive ? "duration-0 bg-primary/30" : string.Empty;
        var disabledClasses = isDisabled ? "cursor-not-allowed opacity-50" : string.Empty;
        var baseClasses = string.Join(" ", [
            "flex w-full items-center gap-2 rounded-md px-2 py-1.5 text-sm",
            "outline-none focus-visible:outline-none transition-colors",
        ]);
        var interactiveClasses = isDisabled ? string.Empty : "cursor-pointer";
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

    private bool IsSelected(SelectDeclarativeNode node)
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

    private bool TryGetNodeValue(SelectDeclarativeNode node, out T? typedValue)
    {
        if (node.Value is null)
        {
            typedValue = default;
            return true;
        }

        if (node.Value is T value)
        {
            typedValue = value;
            return true;
        }

        typedValue = default;
        return false;
    }

    private string? GetActiveDescendant()
    {
        if (!_open || _activeIndex is null)
            return null;

        return GetOptionId(_activeIndex.Value);
    }

    private string GetOptionId(int index) => $"{_listboxId}-option-{index}";

    private List<SelectDeclarativeNode> GetRenderNodes()
    {
        var declarativeNodes = _declarativeRegistry.Snapshot();
        var programmaticNodes = Items?.ToList() ?? [];

        if (declarativeNodes.Count > 0 && programmaticNodes.Count > 0)
        {
            throw new InvalidOperationException("Select cannot use both Items and declarative child items at the same time. Use one mode only.");
        }

        if (declarativeNodes.Count > 0)
            return [.. declarativeNodes];

        return [.. programmaticNodes.Select(x => new SelectDeclarativeNode
        {
            Kind = SelectNodeKind.Item,
            Value = x.Value,
            Text = x.DisplayText,
            Disabled = x.Disabled,
        })];
    }

    private static bool IsSelectable(List<SelectDeclarativeNode> nodes, int index)
    {
        return index >= 0
               && index < nodes.Count
               && nodes[index].Kind == SelectNodeKind.Item
               && !nodes[index].Disabled;
    }

    private static int? GetSelectedIndex(List<SelectDeclarativeNode> nodes, Func<SelectDeclarativeNode, bool> isSelected)
    {
        for (var i = 0; i < nodes.Count; i++)
        {
            if (isSelected(nodes[i]))
                return i;
        }

        return null;
    }

    private static int? GetFirstSelectableIndex(List<SelectDeclarativeNode> nodes)
    {
        for (var i = 0; i < nodes.Count; i++)
        {
            if (IsSelectable(nodes, i))
                return i;
        }

        return null;
    }

    private static int? GetLastSelectableIndex(List<SelectDeclarativeNode> nodes)
    {
        for (var i = nodes.Count - 1; i >= 0; i--)
        {
            if (IsSelectable(nodes, i))
                return i;
        }

        return null;
    }

    private static int? GetNextSelectableIndex(List<SelectDeclarativeNode> nodes, int? current, int steps = 1)
    {
        if (nodes.Count == 0)
            return null;

        var index = current ?? -1;
        var moveCount = Math.Max(1, steps);

        for (var move = 0; move < moveCount; move++)
        {
            var candidate = index + 1;
            while (candidate < nodes.Count && !IsSelectable(nodes, candidate))
            {
                candidate++;
            }

            if (candidate >= nodes.Count)
            {
                break;
            }

            index = candidate;
        }

        if (index == -1)
            return GetFirstSelectableIndex(nodes);

        return index;
    }

    private static int? GetPreviousSelectableIndex(List<SelectDeclarativeNode> nodes, int? current, int steps = 1)
    {
        if (nodes.Count == 0)
            return null;

        var index = current ?? nodes.Count;
        var moveCount = Math.Max(1, steps);

        for (var move = 0; move < moveCount; move++)
        {
            var candidate = index - 1;
            while (candidate >= 0 && !IsSelectable(nodes, candidate))
            {
                candidate--;
            }

            if (candidate < 0)
            {
                break;
            }

            index = candidate;
        }

        if (index == nodes.Count)
            return GetLastSelectableIndex(nodes);

        return index;
    }

    private void SetActiveIndex(int? index)
    {
        _activeIndex = index;
        if (_open && index is not null)
        {
            _pendingScrollOptionId = GetOptionId(index.Value);
        }
    }

    private void HandleOptionMouseEnter(int index)
    {
        var nodes = GetRenderNodes();
        if (!IsSelectable(nodes, index))
            return;

        SetActiveIndex(index);
    }

    private SelectOption<T> ToSelectOption(SelectDeclarativeNode node)
    {
        if (!TryGetNodeValue(node, out var value))
            return new SelectOption<T>(default!, node.Text, node.Disabled);

        return new SelectOption<T>(value!, node.Text, node.Disabled);
    }

    private void EnsureActiveOnOpen()
    {
        var nodes = GetRenderNodes();
        if (nodes.Count == 0)
        {
            SetActiveIndex(null);
            return;
        }

        var selectedIndex = GetSelectedIndex(nodes, IsSelected);
        if (selectedIndex is not null && IsSelectable(nodes, selectedIndex.Value))
        {
            SetActiveIndex(selectedIndex);
            return;
        }

        SetActiveIndex(GetFirstSelectableIndex(nodes));
    }

    private Task HandleOpenChanged(bool open)
    {
        _open = open;
        if (_open)
        {
            EnsureActiveOnOpen();
        }

        return Task.CompletedTask;
    }

    private Task ToggleOpen()
    {
        if (Disabled)
            return Task.CompletedTask;

        _open = !_open;
        if (_open)
        {
            EnsureActiveOnOpen();
        }

        return Task.CompletedTask;
    }

    private async Task HandleTriggerKeyDown(KeyboardEventArgs e)
    {
        if (Disabled)
            return;

        if (string.Equals(e.Key, "Tab", StringComparison.Ordinal))
        {
            await HandleTabKeyAsync(e.ShiftKey);
            return;
        }

        var nodes = GetRenderNodes();
        if (nodes.Count == 0)
            return;

        if (GetFirstSelectableIndex(nodes) is null)
            return;

        // Single-character typeahead: jump to the next enabled option whose text starts with the typed letter.
        if (TryHandleFirstLetterTypeahead(e.Key, nodes))
        {
            if (!_open)
            {
                _open = true;
            }

            await InvokeAsync(StateHasChanged);
            return;
        }

        switch (e.Key)
        {
            case "ArrowDown":
            case "Down":
                if (!_open)
                {
                    _open = true;
                    EnsureActiveOnOpen();
                }
                else
                {
                    SetActiveIndex(GetNextSelectableIndex(nodes, _activeIndex));
                }
                break;
            case "ArrowUp":
            case "Up":
                if (!_open)
                {
                    _open = true;
                    EnsureActiveOnOpen();
                }
                else
                {
                    SetActiveIndex(GetPreviousSelectableIndex(nodes, _activeIndex));
                }
                break;
            case "Home":
                if (!_open)
                    _open = true;
                SetActiveIndex(GetFirstSelectableIndex(nodes));
                break;
            case "End":
                if (!_open)
                    _open = true;
                SetActiveIndex(GetLastSelectableIndex(nodes));
                break;
            case "PageDown":
                if (!_open)
                {
                    _open = true;
                    EnsureActiveOnOpen();
                }
                SetActiveIndex(GetNextSelectableIndex(nodes, _activeIndex, PageJumpSize));
                break;
            case "PageUp":
                if (!_open)
                {
                    _open = true;
                    EnsureActiveOnOpen();
                }
                SetActiveIndex(GetPreviousSelectableIndex(nodes, _activeIndex, PageJumpSize));
                break;
            case "Enter":
            case " ":
                if (!_open)
                {
                    _open = true;
                    EnsureActiveOnOpen();
                }
                else if (_activeIndex is not null)
                {
                    await SelectOptionAtIndexAsync(_activeIndex.Value);
                }
                break;
            case "Escape":
            case "Esc":
                _open = false;
                break;
        }

        await InvokeAsync(StateHasChanged);
    }

    private async Task HandleTabKeyAsync(bool shiftKey)
    {
        var nodes = GetRenderNodes();

        if (_open)
        {
            if (_activeIndex is int activeIndex && IsSelectable(nodes, activeIndex))
            {
                await SelectOptionAtIndexAsync(activeIndex);
            }
            else
            {
                _open = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        await EnsureModuleAsync();
        if (_module is not null)
        {
            await _module.InvokeVoidAsync("focusAdjacentFocusable", _triggerId, !shiftKey);
        }
    }

    private bool TryHandleFirstLetterTypeahead(string? key, List<SelectDeclarativeNode> nodes)
    {
        if (string.IsNullOrWhiteSpace(key) || key!.Length != 1)
            return false;

        var ch = key[0];
        if (char.IsControl(ch) || char.IsWhiteSpace(ch))
            return false;

        var start = (_activeIndex ?? -1) + 1;
        for (var offset = 0; offset < nodes.Count; offset++)
        {
            var i = (start + offset) % nodes.Count;
            if (!IsSelectable(nodes, i))
                continue;

            var text = nodes[i].Text;
            if (!string.IsNullOrEmpty(text) &&
                char.ToUpperInvariant(text[0]) == char.ToUpperInvariant(ch))
            {
                SetActiveIndex(i);
                return true;
            }
        }

        return false;
    }

    private async Task SelectOptionAtIndexAsync(int index)
    {
        var nodes = GetRenderNodes();
        if (!IsSelectable(nodes, index))
            return;

        var node = nodes[index];
        if (!TryGetNodeValue(node, out var nodeValue))
        {
            throw new InvalidOperationException($"SelectItem value type '{node.Value?.GetType().Name}' is incompatible with Select<{typeof(T).Name}>.");
        }

        Value = nodeValue;
        SetActiveIndex(index);
        await ValueChanged.InvokeAsync(Value);
        _open = false;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_open && !string.IsNullOrEmpty(_pendingScrollOptionId))
        {
            await EnsureModuleAsync();
            if (_module is not null)
            {
                await _module.InvokeVoidAsync("ensureOptionVisible", _listboxId, _pendingScrollOptionId);
            }
            _pendingScrollOptionId = null;
        }

        if (_open && ShouldApplyMaxVisibleItems() && NeedsMaxVisibleItemsMeasurement())
        {
            await EnsureModuleAsync();
            if (_module is not null && MaxVisibleItems is not null)
            {
                await _module.InvokeVoidAsync("applyListboxMaxVisibleItems", _listboxId, MaxVisibleItems.Value, 96);
                _maxVisibleItemsListenerRegistered = true;
                MarkMaxVisibleItemsMeasured();
            }
        }

        if (!_open && _maxVisibleItemsListenerRegistered)
        {
            await DisposeMaxVisibleItemsListenerAsync();
            ResetMaxVisibleItemsMeasurementState();
        }
    }

    private bool ShouldApplyMaxVisibleItems() => MaxVisibleItems is > 0;

    private bool NeedsMaxVisibleItemsMeasurement()
    {
        var nodeCount = GetRenderNodes().Count;
        return !_maxVisibleItemsListenerRegistered
               || _lastMeasuredNodeCount != nodeCount
               || _lastMeasuredMaxVisibleItems != MaxVisibleItems;
    }

    private void MarkMaxVisibleItemsMeasured()
    {
        _lastMeasuredNodeCount = GetRenderNodes().Count;
        _lastMeasuredMaxVisibleItems = MaxVisibleItems;
    }

    private void ResetMaxVisibleItemsMeasurementState()
    {
        _maxVisibleItemsListenerRegistered = false;
        _lastMeasuredNodeCount = -1;
        _lastMeasuredMaxVisibleItems = null;
    }

    private async Task DisposeMaxVisibleItemsListenerAsync()
    {
        if (_module is null)
            return;

        await _module.InvokeVoidAsync("disposeListboxMaxVisibleItems", _listboxId);
    }

    private async Task EnsureModuleAsync()
    {
        if (_module is not null)
            return;

        _module = await JsRuntime.InvokeAsync<IJSObjectReference>(
            "import",
            "/ShadcnBlazor/_content/ShadcnBlazor/Components/Select/Select.razor.js");
    }

    public async ValueTask DisposeAsync()
    {
        _declarativeRegistry.Changed -= HandleDeclarativeChanged;

        if (_maxVisibleItemsListenerRegistered)
        {
            await DisposeMaxVisibleItemsListenerAsync();
            ResetMaxVisibleItemsMeasurementState();
        }

        if (_module is not null)
        {
            await _module.DisposeAsync();
            _module = null;
        }
    }
}





