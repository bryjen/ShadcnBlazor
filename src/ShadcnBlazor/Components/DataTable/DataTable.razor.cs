using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace ShadcnBlazor.Components.DataTable;

public enum DataTableInteractionMode
{
    Table,
    Grid,
}

file sealed class ObjectComparer : IComparer<object?>
{
    public static readonly ObjectComparer Instance = new();

    public int Compare(object? x, object? y)
    {
        if (x is null && y is null) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        if (x is string sx && y is string sy)
            return string.Compare(sx, sy, StringComparison.OrdinalIgnoreCase);

        if (x is IComparable cx)
            return cx.CompareTo(y);

        return string.Compare(x.ToString(), y.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}

public partial class DataTable<T> : ComponentBase
{
    private DataTableContext<T> _context = new();
    private int _currentPage = 0;
    private int _currentPageSize;
    private DataTableColumn<T>? _sortCol;
    private bool _sortDesc;
    private int? _focusedRowAbsoluteIndex;
    private int? _pendingFocusAbsoluteIndex;

    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    [Parameter] public IReadOnlyList<T> Items { get; set; } = [];
    [Parameter] public RenderFragment? Columns { get; set; }
    [Parameter] public RenderFragment? ToolBarContent { get; set; }
    [Parameter] public RenderFragment? EmptyContent { get; set; }
    [Parameter] public bool IsLoading { get; set; }
    [Parameter] public EventCallback<T> OnRowClick { get; set; }
    [Parameter] public bool Dense { get; set; }
    [Parameter] public bool Hover { get; set; } = true;
    [Parameter] public bool Striped { get; set; }
    [Parameter] public int PageSize { get; set; } = 10;
    [Parameter] public int[] PageSizeOptions { get; set; } = [5, 10, 25, 50];
    [Parameter] public string? Label { get; set; }
    [Parameter] public DataTableInteractionMode InteractionMode { get; set; }
    [Parameter] public bool EnableDebugLogging { get; set; }

    // Multi-selection
    [Parameter] public bool MultiSelection { get; set; }
    [Parameter] public HashSet<T> SelectedItems { get; set; } = [];
    [Parameter] public EventCallback<HashSet<T>> SelectedItemsChanged { get; set; }

    protected override void OnInitialized()
    {
        _currentPageSize = PageSize;
        _context.Changed += () => InvokeAsync(StateHasChanged);
    }

    protected override void OnParametersSet()
    {
        _currentPage = 0;
        EnsureFocusedRowForCurrentPage(requestFocus: false);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_pendingFocusAbsoluteIndex is not int absolute)
            return;

        _pendingFocusAbsoluteIndex = null;
        await FocusRowAsync(absolute);
    }

    private IEnumerable<T> SortedItems
    {
        get
        {
            if (_sortCol?.Field is null) return Items;
            return _sortDesc
                ? Items.OrderByDescending(x => _sortCol.Field(x), ObjectComparer.Instance)
                : Items.OrderBy(x => _sortCol.Field(x), ObjectComparer.Instance);
        }
    }

    private IEnumerable<T> PagedItems =>
        SortedItems.Skip(_currentPage * _currentPageSize).Take(_currentPageSize);

    private bool IsGridMode => InteractionMode == DataTableInteractionMode.Grid;

    private int DataRowCount => IsLoading ? _currentPageSize : Items.Count;

    private int ColumnCount => _context.Columns.Count + (MultiSelection ? 1 : 0);

    private string EffectiveLabel =>
        string.IsNullOrWhiteSpace(Label) ? "Data table" : Label!;

    private int TotalPages =>
        (int)Math.Ceiling((double)Items.Count / _currentPageSize);

    private int GetCurrentPageItemCount() => IsLoading ? 0 : PagedItems.Count();

    private int GetPageFirstRowAbsoluteIndex() => _currentPage * _currentPageSize + 1;

    private int GetRowAbsoluteIndex(int localIdx) => GetPageFirstRowAbsoluteIndex() + localIdx;

    private int GetLocalIndex(int absoluteIndex) => absoluteIndex - GetPageFirstRowAbsoluteIndex();

    private string GetRowId(int absoluteRowIndex) => $"datatable-row-{GetHashCode()}-{absoluteRowIndex}";

    private int? GetRowTabIndex(int localIdx)
    {
        if (!IsGridMode) return null;

        var absoluteIndex = GetRowAbsoluteIndex(localIdx);
        if (_focusedRowAbsoluteIndex is null)
            return localIdx == 0 ? 0 : -1;

        return _focusedRowAbsoluteIndex == absoluteIndex ? 0 : -1;
    }

    private string? GetTableRole() => IsGridMode ? "grid" : null;

    private string? GetRowRole() => IsGridMode ? "row" : null;

    private string? GetColumnHeaderRole() => IsGridMode ? "columnheader" : null;

    private string? GetGridCellRole() => IsGridMode ? "gridcell" : null;

    private int? GetColIndex(int zeroBasedVisibleColIndex) => IsGridMode ? zeroBasedVisibleColIndex + 1 : null;

    private void LogDebug(string message)
    {
        if (!EnableDebugLogging)
            return;

        var full = $"[DataTable] {message}";
        Console.WriteLine(full);
        _ = JsRuntime.InvokeVoidAsync("console.debug", full);
    }

    private async Task FocusRowAsync(int absoluteRowIndex)
    {
        if (!IsGridMode)
            return;

        var rowId = GetRowId(absoluteRowIndex);
        await JsRuntime.InvokeVoidAsync("eval", $"document.getElementById('{rowId}')?.focus()");
        LogDebug($"Focused row abs={absoluteRowIndex} id={rowId}");
    }

    private void EnsureFocusedRowForCurrentPage(bool requestFocus)
    {
        if (!IsGridMode || IsLoading)
        {
            _focusedRowAbsoluteIndex = null;
            _pendingFocusAbsoluteIndex = null;
            return;
        }

        var count = GetCurrentPageItemCount();
        if (count == 0)
        {
            _focusedRowAbsoluteIndex = null;
            _pendingFocusAbsoluteIndex = null;
            return;
        }

        var first = GetPageFirstRowAbsoluteIndex();
        var last = first + count - 1;

        if (_focusedRowAbsoluteIndex is null || _focusedRowAbsoluteIndex < first || _focusedRowAbsoluteIndex > last)
            _focusedRowAbsoluteIndex = first;

        if (requestFocus)
            _pendingFocusAbsoluteIndex = _focusedRowAbsoluteIndex;

        LogDebug($"EnsureFocus page={_currentPage + 1} count={count} focusAbs={_focusedRowAbsoluteIndex}");
    }

    private async Task MoveRowFocusWithinCurrentPageAsync(int targetLocalIdx)
    {
        var count = GetCurrentPageItemCount();
        if (!IsGridMode || count == 0)
            return;

        var clampedLocal = Math.Clamp(targetLocalIdx, 0, count - 1);
        var absoluteIndex = GetRowAbsoluteIndex(clampedLocal);
        _focusedRowAbsoluteIndex = absoluteIndex;
        _pendingFocusAbsoluteIndex = absoluteIndex;

        LogDebug($"MoveFocus targetLocal={targetLocalIdx} clampedLocal={clampedLocal} abs={absoluteIndex}");

        await InvokeAsync(StateHasChanged);
    }

    private void OnHeaderClick(DataTableColumn<T> col)
    {
        if (col.Sortable)
            ToggleSort(col);
    }

    private void OnHeaderKeyDown(KeyboardEventArgs args, DataTableColumn<T> col)
    {
        if (!col.Sortable)
            return;

        if (args.Key is "Enter" or " " or "Spacebar")
            ToggleSort(col);
    }

    private void ToggleSort(DataTableColumn<T> col)
    {
        if (_sortCol == col)
            _sortDesc = !_sortDesc;
        else
        {
            _sortCol = col;
            _sortDesc = false;
        }

        _currentPage = 0;
        EnsureFocusedRowForCurrentPage(requestFocus: false);
        LogDebug($"Sort changed col='{col.Title}' dir={(_sortDesc ? "desc" : "asc")}");
    }

    private string? GetSortAriaValue(DataTableColumn<T> col)
    {
        if (!col.Sortable) return null;
        if (_sortCol != col) return "none";
        return _sortDesc ? "descending" : "ascending";
    }

    private void OnPageSizeChanged(int size)
    {
        _currentPageSize = size;
        _currentPage = 0;
        EnsureFocusedRowForCurrentPage(requestFocus: true);
    }

    private void FirstPage()
    {
        _currentPage = 0;
        EnsureFocusedRowForCurrentPage(requestFocus: true);
    }

    private void PrevPage()
    {
        if (_currentPage > 0)
            _currentPage--;

        EnsureFocusedRowForCurrentPage(requestFocus: true);
    }

    private void NextPage()
    {
        if (_currentPage < TotalPages - 1)
            _currentPage++;

        EnsureFocusedRowForCurrentPage(requestFocus: true);
    }

    private void LastPage()
    {
        _currentPage = Math.Max(0, TotalPages - 1);
        EnsureFocusedRowForCurrentPage(requestFocus: true);
    }

    private void OnRowFocus(int localIdx)
    {
        if (!IsGridMode)
            return;

        _focusedRowAbsoluteIndex = GetRowAbsoluteIndex(localIdx);
        LogDebug($"Row focused local={localIdx} abs={_focusedRowAbsoluteIndex}");
    }

    private async Task OnRowKeyDown(KeyboardEventArgs args, T item, int localIdx)
    {
        if (!IsGridMode)
            return;

        var count = GetCurrentPageItemCount();
        if (count == 0)
            return;

        var currentAbs = _focusedRowAbsoluteIndex ?? GetRowAbsoluteIndex(localIdx);
        var currentLocal = Math.Clamp(GetLocalIndex(currentAbs), 0, count - 1);

        LogDebug($"KeyDown key='{args.Key}' currentLocal={currentLocal} currentAbs={currentAbs}");

        switch (args.Key)
        {
            case "ArrowDown":
                await MoveRowFocusWithinCurrentPageAsync(currentLocal + 1);
                break;
            case "ArrowUp":
                await MoveRowFocusWithinCurrentPageAsync(currentLocal - 1);
                break;
            case "Home":
                await MoveRowFocusWithinCurrentPageAsync(0);
                break;
            case "End":
                await MoveRowFocusWithinCurrentPageAsync(count - 1);
                break;
            case "Enter":
                await OnRowClick.InvokeAsync(item);
                LogDebug("Enter invoked OnRowClick");
                break;
            case " ":
            case "Spacebar":
                if (MultiSelection)
                {
                    await ToggleItem(item);
                    LogDebug("Space toggled row selection");
                }
                else
                {
                    await OnRowClick.InvokeAsync(item);
                    LogDebug("Space invoked OnRowClick");
                }
                break;
        }
    }

    // Multi-selection helpers
    private bool IsSelected(T item) => SelectedItems.Contains(item);

    private bool AllSelected =>
        Items.Count > 0 && Items.All(x => SelectedItems.Contains(x));

    private async Task ToggleItem(T item)
    {
        var next = new HashSet<T>(SelectedItems);
        if (!next.Add(item)) next.Remove(item);
        SelectedItems = next;
        await SelectedItemsChanged.InvokeAsync(SelectedItems);
        LogDebug($"Selection changed count={SelectedItems.Count}");
    }

    private async Task ToggleAll()
    {
        SelectedItems = AllSelected ? [] : [.. Items];
        await SelectedItemsChanged.InvokeAsync(SelectedItems);
        LogDebug($"ToggleAll selected={SelectedItems.Count}");
    }

    private async Task HandleRowClick(T item, int localIdx)
    {
        if (IsGridMode)
        {
            _focusedRowAbsoluteIndex = GetRowAbsoluteIndex(localIdx);
            _pendingFocusAbsoluteIndex = _focusedRowAbsoluteIndex;
            LogDebug($"Row click set focus abs={_focusedRowAbsoluteIndex}");
        }

        if (MultiSelection)
            await ToggleItem(item);

        await OnRowClick.InvokeAsync(item);
    }
}
