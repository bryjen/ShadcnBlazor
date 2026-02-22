using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Components.Select.Base;

public abstract partial class SelectBase<T> : ShadcnComponentBase
{
    [Inject] 
    public IJSRuntime JsRuntime { get; set; } = null!;
    
#region Programmatic
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
#endregion

    protected abstract Task OnItemSelect(int idx, T value);
    
    private readonly SelectDeclarativeRegistry _declarativeRegistry = new();
    private readonly string _triggerId = $"select-trigger-{Guid.NewGuid():N}";
    private readonly string _listboxId = $"select-listbox-{Guid.NewGuid():N}";
    
    /// <remarks>
    /// Picking either the declarative or programmatic approach flattens everything down to <see cref="SelectDeclarativeNode"/> objects.
    /// This is the ccommon interface; How these are rendered is delegated to inheritors.
    /// <br/>
    /// In terms of rendering, in the declarative approach, each <b>item</b> node is expected to bring its own render fragment.
    /// In the programmatic approach, this is not the case. A single <see cref="RenderItem"/> is used to render all items,
    /// and the individual node only carries the data.
    /// </remarks>
    private List<SelectDeclarativeNode> GetRenderNodes()
    {
        var declarativeNodes = _declarativeRegistry.Snapshot();
        var programmaticNodes = Items.ToList();

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
            Render = RenderItem is not null ? builder => RenderItem(x)(builder) : null
        })];
    }
    
#region Selection
    /* Opinionated selection logic. 
     */

    /// <summary>
    /// Determines whether a node is selectable.
    /// </summary>
    /// <remarks>
    /// Recall that nodes can include group names and separators. This essentially asserts that the node is an item
    /// and in a non-disabled state.
    /// </remarks>
    private static bool IsSelectable(List<SelectDeclarativeNode> nodes, int index)
    {
        return index >= 0
               && index < nodes.Count
               && nodes[index].Kind == SelectNodeKind.Item
               && !nodes[index].Disabled;
    }
    
    /// self-explanatory
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

    private async Task SelectOptionAtIndexAsync(int index)
    {
        var nodes = GetRenderNodes();
        if (!IsSelectable(nodes, index))
            return;

        var node = nodes[index];
        if (!TryGetNodeValue(node, out var nodeValue) || nodeValue is null)
        {
            throw new InvalidOperationException($"SelectItem value type '{node.Value?.GetType().Name}' is incompatible with Select<{typeof(T).Name}>.");
        }

        await OnItemSelect(index, nodeValue);
        /*
        Value = nodeValue;
        SetActiveIndex(index);
        await ValueChanged.InvokeAsync(Value);
        _open = false;
         */
    }
#endregion

#region Scroll
    private string? _pendingScrollOptionId;
    private IJSObjectReference? _module;
    private bool _maxVisibleItemsListenerRegistered;
    private int _lastMeasuredNodeCount = -1;
    private int? _lastMeasuredMaxVisibleItems;
    private const int PageJumpSize = 10;

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
            "/ShadcnBlazor/_content/ShadcnBlazor/Components/Select/Base/SelectBase.razor.js");
    }
#endregion
}