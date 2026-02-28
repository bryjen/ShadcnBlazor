using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Components.Select.Base;

public abstract partial class SelectBase<T> : ShadcnComponentBase, IAsyncDisposable
{
    [Inject]
    public IJSRuntime JsRuntime { get; set; } = null!;

#region Parameters
    /// <summary>
    /// Programmatic list of options to render. Do not use together with declarative child items.
    /// </summary>
    [Parameter]
    public IEnumerable<SelectOption<T>> Items { get; set; } = [];

    /// <summary>
    /// Custom template used to render each selectable item.
    /// </summary>
    [Parameter]
    public RenderFragment<SelectOption<T>>? RenderItem { get; set; }

    /// <summary>
    /// Maximum number of visible options before the list becomes scrollable. When null or less than 1, no item-count cap is applied.
    /// </summary>
    [Parameter]
    public int? MaxVisibleItems { get; set; }

    /// <summary>
    /// Declarative option content (for example SelectItem, SelectLabel, SelectSeparator, SelectGroup).
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Disables the trigger and prevents interaction.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }
#endregion

    protected readonly SelectDeclarativeRegistry _declarativeRegistry = new();
    protected readonly string _triggerId = $"select-trigger-{Guid.NewGuid():N}";
    protected readonly string _listboxId = $"select-listbox-{Guid.NewGuid():N}";

    protected bool _open;
    protected int? _activeIndex;
    protected string? _pendingScrollOptionId;

    protected IJSObjectReference? _module;
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

    /// <remarks>
    /// Picking either the declarative or programmatic approach flattens everything down to <see cref="SelectDeclarativeNode"/> objects.
    /// This is the common interface.
    /// <br/>
    /// In the declarative approach, each item node is expected to bring its own render fragment.
    /// In the programmatic approach, a single <see cref="RenderItem"/> is used to render all items.
    /// </remarks>
    protected virtual List<SelectDeclarativeNode> GetRenderNodes()
    {
        var declarativeNodes = _declarativeRegistry.Snapshot();
        var programmaticNodes = Items.ToList();

        if (declarativeNodes.Count > 0 && programmaticNodes.Count > 0)
        {
            throw new InvalidOperationException(
                "Select cannot use both Items and declarative child items at the same time. Use one mode only.");
        }

        if (declarativeNodes.Count > 0)
            return [.. declarativeNodes];

        return [.. programmaticNodes.Select(x => new SelectDeclarativeNode
        {
            Kind = SelectNodeKind.Item,
            Value = x.Value,
            Text = x.DisplayText,
            Disabled = x.Disabled,
            Render = RenderItem is not null ? builder => RenderItem(x)(builder) : null
        })];
    }

#region Selection
    protected abstract bool IsSelected(SelectDeclarativeNode node);

    protected bool TryGetNodeValue(SelectDeclarativeNode node, out T? typedValue)
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

    private static bool IsSelectable(List<SelectDeclarativeNode> nodes, int index)
    {
        return index >= 0
               && index < nodes.Count
               && (nodes[index].Kind == SelectNodeKind.Item || nodes[index].Kind == SelectNodeKind.Action)
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
#endregion

#region Active
    protected string? GetActiveDescendant()
    {
        if (!_open || _activeIndex is null)
            return null;

        return GetOptionId(_activeIndex.Value);
    }

    protected string GetOptionId(int index) => $"{_listboxId}-option-{index}";

    protected void SetActiveIndex(int? index)
    {
        _activeIndex = index;
        if (_open && index is not null)
        {
            _pendingScrollOptionId = GetOptionId(index.Value);
        }
    }

    protected void HandleOptionMouseEnter(int index)
    {
        var nodes = GetRenderNodes();
        if (!IsSelectable(nodes, index))
            return;

        SetActiveIndex(index);
    }

    protected virtual void EnsureActiveOnOpen()
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
#endregion

#region Open
    protected virtual Task HandleOpenChanged(bool open)
    {
        _open = open;
        if (_open)
        {
            EnsureActiveOnOpen();
        }

        return Task.CompletedTask;
    }

    protected Task ToggleOpen()
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
#endregion

#region Keyboard
    protected async Task HandleTriggerKeyDown(KeyboardEventArgs e)
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

    protected virtual async Task HandleTabKeyAsync(bool shiftKey)
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
            if (!string.IsNullOrEmpty(text)
                && char.ToUpperInvariant(text[0]) == char.ToUpperInvariant(ch))
            {
                SetActiveIndex(i);
                return true;
            }
        }

        return false;
    }
#endregion

#region Commit
    protected virtual async Task OnItemSelectAsync(int idx, T? value)
    {
        SetActiveIndex(idx);
        _open = false;
        await Task.CompletedTask;
    }

    protected async Task SelectOptionAtIndexAsync(int index)
    {
        var nodes = GetRenderNodes();
        if (!IsSelectable(nodes, index))
            return;

        var node = nodes[index];

        if (node.Kind == SelectNodeKind.Action)
        {
            if (node.Callback is not null)
                await node.Callback();
            return;
        }

        if (!TryGetNodeValue(node, out var nodeValue))
        {
            throw new InvalidOperationException(
                $"SelectItem value type '{node.Value?.GetType().Name}' is incompatible with Select<{typeof(T).Name}>.");
        }

        await OnItemSelectAsync(index, nodeValue);
    }
#endregion

#region JS Interop
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

    protected async Task EnsureModuleAsync()
    {
        if (_module is not null)
            return;

        _module = await JsRuntime.InvokeAsync<IJSObjectReference>(
            "import",
            "/_content/ShadcnBlazor/Components/Select/Base/SelectBase.razor.js");
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
#endregion
}