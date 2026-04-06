using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Components.DataTable;

/// <summary>Describes a page request sent to a server-side data provider.</summary>
public sealed record DataTableRequest(int Page, int PageSize, string? SortColumn, bool SortDescending);

/// <summary>The result returned by a server-side data provider.</summary>
public sealed record DataTableResult<T>(IReadOnlyList<T> Items, int TotalCount);

/// <summary>Defines the interaction model used by the data table.</summary>
public enum DataTableInteractionMode
{
    /// <summary>Standard table semantics and interaction.</summary>
    Table,
    /// <summary>Grid semantics with focusable row navigation.</summary>
    Grid,
}

/// <summary>Provides pagination state and navigation methods for custom pagination UI.</summary>
public sealed class DataTablePaginationContext
{
    /// <summary>Current zero-based page index.</summary>
    public int CurrentPage { get; init; }
    /// <summary>Number of items per page.</summary>
    public int PageSize { get; init; }
    /// <summary>Total number of pages.</summary>
    public int TotalPages { get; init; }
    /// <summary>Total number of items across all pages.</summary>
    public int TotalItems { get; init; }
    /// <summary>Number of currently selected items (when MultiSelection is enabled).</summary>
    public int SelectedCount { get; init; }
    /// <summary>True if currently on the first page.</summary>
    public bool IsFirstPage { get; init; }
    /// <summary>True if currently on the last page.</summary>
    public bool IsLastPage { get; init; }
    /// <summary>Available page size options.</summary>
    public int[] PageSizeOptions { get; init; } = [];

    /// <summary>Navigate to the first page.</summary>
    public Func<Task> FirstPageAsync { get; init; } = null!;
    /// <summary>Navigate to the previous page.</summary>
    public Func<Task> PrevPageAsync { get; init; } = null!;
    /// <summary>Navigate to the next page.</summary>
    public Func<Task> NextPageAsync { get; init; } = null!;
    /// <summary>Navigate to the last page.</summary>
    public Func<Task> LastPageAsync { get; init; } = null!;
    /// <summary>Navigate to a specific zero-based page index.</summary>
    public Func<int, Task> GoToPageAsync { get; init; } = null!;
    /// <summary>Change the page size.</summary>
    public Func<int, Task> ChangePageSizeAsync { get; init; } = null!;
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

/// <summary>Displays data in tabular or grid form with sorting, paging, and optional server mode.</summary>
public partial class DataTable<T> : ShadcnComponentBase
{
    private DataTableContext<T> _context = new();
    private int _currentPage = 0;
    private int _currentPageSize;
    private DataTableColumn<T>? _sortCol;
    private bool _sortDesc;
    private int? _focusedRowAbsoluteIndex;
    private int? _pendingFocusAbsoluteIndex;

    // Server-mode state
    private IReadOnlyList<T> _serverItems = [];
    private int _serverTotalCount;
    private bool _isServerLoading;

    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    /// <summary>Items for client-side mode. Ignored when <see cref="DataProvider"/> is set.</summary>
    [Parameter]
    [Category(ComponentCategory.Items)]
    public IReadOnlyList<T> Items { get; set; } = [];

    /// <summary>
    /// When set, the table operates in server-side mode. The delegate is called whenever the
    /// page, page size, or sort changes. <see cref="Items"/> is ignored in this mode.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Items)]
    public Func<DataTableRequest, Task<DataTableResult<T>>>? DataProvider { get; set; }

    /// <summary>Column definitions.</summary>
    [Parameter]
    [Category(ComponentCategory.Content)]
    public RenderFragment? Columns { get; set; }
    /// <summary>Optional toolbar content rendered above the table.</summary>
    [Parameter]
    [Category(ComponentCategory.Content)]
    public RenderFragment? ToolBarContent { get; set; }
    /// <summary>Content displayed when there are no rows.</summary>
    [Parameter]
    [Category(ComponentCategory.Content)]
    public RenderFragment? EmptyContent { get; set; }
    /// <summary>Custom pagination content. When provided, replaces the default pagination footer.</summary>
    [Parameter]
    [Category(ComponentCategory.Content)]
    public RenderFragment<DataTablePaginationContext>? PaginationContent { get; set; }
    /// <summary>Shows loading placeholders.</summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public bool IsLoading { get; set; }
    /// <summary>Callback invoked when a row is clicked or activated.</summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public EventCallback<T> OnRowClick { get; set; }
    /// <summary>Renders a compact table layout.</summary>
    [Parameter]
    [Category(ComponentCategory.Appearance)]
    public bool Dense { get; set; }
    /// <summary>Enables row hover styling.</summary>
    [Parameter]
    [Category(ComponentCategory.Appearance)]
    public bool Hover { get; set; } = true;
    /// <summary>Alternates row striping.</summary>
    [Parameter]
    [Category(ComponentCategory.Appearance)]
    public bool Striped { get; set; }
    /// <summary>Number of rows per page.</summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public int PageSize { get; set; } = 10;
    /// <summary>Available page-size options.</summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public int[] PageSizeOptions { get; set; } = [5, 10, 25, 50];
    /// <summary>Accessible label for the table.</summary>
    [Parameter]
    [Category(ComponentCategory.Common)]
    public string? Label { get; set; }
    /// <summary>Interaction model for the table.</summary>
    [Parameter]
    [Category(ComponentCategory.Appearance)]
    public DataTableInteractionMode InteractionMode { get; set; }
    /// <summary>Writes debug information to the console.</summary>
    [Parameter]
    [Category(ComponentCategory.Common)]
    public bool EnableDebugLogging { get; set; }
    /// <summary>Shows the pagination footer. Defaults to true.</summary>
    [Parameter]
    [Category(ComponentCategory.Appearance)]
    public bool ShowPagination { get; set; } = true;

    // Multi-selection
    /// <summary>Enables multi-row selection.</summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public bool MultiSelection { get; set; }
    /// <summary>Currently selected items.</summary>
    [Parameter]
    [Category(ComponentCategory.Data)]
    public HashSet<T> SelectedItems { get; set; } = [];
    /// <summary>Callback invoked when the selection changes.</summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public EventCallback<HashSet<T>> SelectedItemsChanged { get; set; }

    private bool IsServerMode => DataProvider is not null;
    private bool ShowSkeleton => IsLoading || _isServerLoading;

    [Parameter]
    [Category(ComponentCategory.Appearance)]
    public string OuterTableContainerClass { get; set; } = string.Empty;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        _currentPageSize = PageSize;
        _context.Changed += () => InvokeAsync(StateHasChanged);
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        if (IsServerMode)
            await FetchFromProviderAsync();
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        // In server mode the parent controls resets via ResetAsync(); don't clobber the page.
        if (!IsServerMode)
            _currentPage = 0;

        EnsureFocusedRowForCurrentPage(requestFocus: false);
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_pendingFocusAbsoluteIndex is not int absolute)
            return;

        _pendingFocusAbsoluteIndex = null;
        await FocusRowAsync(absolute);
    }

    /// <summary>
    /// Resets to page 0 and, in server mode, re-fetches from the provider.
    /// Call this from the parent when the underlying data changes (e.g. after a search or sync).
    /// </summary>
    public async Task ResetAsync()
    {
        _currentPage = 0;
        if (IsServerMode)
            await FetchFromProviderAsync();
        else
            await InvokeAsync(StateHasChanged);
    }

    private async Task FetchFromProviderAsync()
    {
        _isServerLoading = true;
        await InvokeAsync(StateHasChanged);
        try
        {
            var req = new DataTableRequest(_currentPage, _currentPageSize, _sortCol?.SortKey, _sortDesc);
            var result = await DataProvider!(req);
            _serverItems = result.Items;
            _serverTotalCount = result.TotalCount;
        }
        finally
        {
            _isServerLoading = false;
        }
        EnsureFocusedRowForCurrentPage(requestFocus: false);
        await InvokeAsync(StateHasChanged);
    }

    private IEnumerable<T> SortedItems
    {
        get
        {
            if (IsServerMode) return _serverItems;
            if (_sortCol?.Field is null) return Items;
            return _sortDesc
                ? Items.OrderByDescending(x => _sortCol.Field(x), ObjectComparer.Instance)
                : Items.OrderBy(x => _sortCol.Field(x), ObjectComparer.Instance);
        }
    }

    private IEnumerable<T> PagedItems =>
        IsServerMode
            ? _serverItems
            : SortedItems.Skip(_currentPage * _currentPageSize).Take(_currentPageSize);

    private bool IsGridMode => InteractionMode == DataTableInteractionMode.Grid;

    private int DataRowCount => ShowSkeleton ? _currentPageSize : (IsServerMode ? _serverItems.Count : Items.Count);

    private int ColumnCount => _context.Columns.Count + (MultiSelection ? 1 : 0);

    private string EffectiveLabel =>
        string.IsNullOrWhiteSpace(Label) ? "Data table" : Label!;

    private int TotalPages =>
        IsServerMode
            ? (int)Math.Ceiling((double)_serverTotalCount / _currentPageSize)
            : (int)Math.Ceiling((double)Items.Count / _currentPageSize);

    private int GetCurrentPageItemCount() => ShowSkeleton ? 0 : PagedItems.Count();

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
        if (!IsGridMode || ShowSkeleton)
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

    private async Task OnHeaderClick(DataTableColumn<T> col)
    {
        if (col.Sortable)
            await ToggleSort(col);
    }

    private async Task OnHeaderKeyDown(KeyboardEventArgs args, DataTableColumn<T> col)
    {
        if (!col.Sortable)
            return;

        if (args.Key is "Enter" or " " or "Spacebar")
            await ToggleSort(col);
    }

    private async Task ToggleSort(DataTableColumn<T> col)
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

        if (IsServerMode)
            await FetchFromProviderAsync();
    }

    private string? GetSortAriaValue(DataTableColumn<T> col)
    {
        if (!col.Sortable) return null;
        if (_sortCol != col) return "none";
        return _sortDesc ? "descending" : "ascending";
    }

    private async Task OnPageSizeChanged(int size)
    {
        _currentPageSize = size;
        _currentPage = 0;
        EnsureFocusedRowForCurrentPage(requestFocus: true);

        if (IsServerMode)
            await FetchFromProviderAsync();
    }

    private async Task FirstPage()
    {
        _currentPage = 0;
        EnsureFocusedRowForCurrentPage(requestFocus: true);

        if (IsServerMode)
            await FetchFromProviderAsync();
    }

    private async Task PrevPage()
    {
        if (_currentPage > 0)
            _currentPage--;

        EnsureFocusedRowForCurrentPage(requestFocus: true);

        if (IsServerMode)
            await FetchFromProviderAsync();
    }

    private async Task NextPage()
    {
        if (_currentPage < TotalPages - 1)
            _currentPage++;

        EnsureFocusedRowForCurrentPage(requestFocus: true);

        if (IsServerMode)
            await FetchFromProviderAsync();
    }

    private async Task LastPage()
    {
        _currentPage = Math.Max(0, TotalPages - 1);
        EnsureFocusedRowForCurrentPage(requestFocus: true);

        if (IsServerMode)
            await FetchFromProviderAsync();
    }

    private async Task GoToPageAsync(int pageNumber)
    {
        var targetPage = Math.Clamp(pageNumber, 0, Math.Max(0, TotalPages - 1));
        if (targetPage == _currentPage) return;

        _currentPage = targetPage;
        EnsureFocusedRowForCurrentPage(requestFocus: true);

        if (IsServerMode)
            await FetchFromProviderAsync();
        else
            await InvokeAsync(StateHasChanged);
    }

    private DataTablePaginationContext GetPaginationContext() => new()
    {
        CurrentPage = _currentPage,
        PageSize = _currentPageSize,
        TotalPages = TotalPages,
        TotalItems = IsServerMode ? _serverTotalCount : Items.Count,
        SelectedCount = SelectedItems.Count,
        IsFirstPage = _currentPage == 0,
        IsLastPage = _currentPage >= TotalPages - 1,
        PageSizeOptions = PageSizeOptions,
        FirstPageAsync = FirstPage,
        PrevPageAsync = PrevPage,
        NextPageAsync = NextPage,
        LastPageAsync = LastPage,
        GoToPageAsync = GoToPageAsync,
        ChangePageSizeAsync = OnPageSizeChanged
    };

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

        // Only toggle selection on row click if there's no custom row click handler
        if (MultiSelection && !OnRowClick.HasDelegate)
            await ToggleItem(item);

        await OnRowClick.InvokeAsync(item);
    }
}
