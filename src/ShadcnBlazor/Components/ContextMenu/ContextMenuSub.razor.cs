using Microsoft.AspNetCore.Components;

namespace ShadcnBlazor.Components.ContextMenu;

/// <summary>
/// Container for a nested sub-menu within a context menu.
/// </summary>
public partial class ContextMenuSub : ComponentBase, IDisposable
{
    private readonly ContextMenuSubContext _context = new();
    private bool _open;
    private CancellationTokenSource? _openCts;
    private CancellationTokenSource? _closeCts;
    private const int HoverOpenDelayMs = 100;
    private const int HoverCloseDelayMs = 300;

    /// <summary>
    /// The sub-trigger and sub-content.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [CascadingParameter]
    private ContextMenuSubContext? ParentSubContext { get; set; }

    [CascadingParameter]
    private ContextMenuContext? RootMenuContext { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        _context.SetOpen = v =>
        {
            _open = v;
            _context.Open = v;
            InvokeAsync(StateHasChanged);
        };
        _context.CancelClose = CancelClose;
        _context.ParentContext = ParentSubContext;
        _context.RootMenuContext = RootMenuContext;
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        _context.Open = _open;
        _context.ParentContext = ParentSubContext;
        _context.RootMenuContext = RootMenuContext;
    }

    private void HandleTriggerMouseEnter()
    {
        CancelClose();
        ParentSubContext?.CancelClose?.Invoke();
        _openCts = new CancellationTokenSource();
        _ = OpenAfterDelayAsync(_openCts.Token);
    }

    private void HandleTriggerMouseLeave()
    {
        CancelOpen();
        _closeCts = new CancellationTokenSource();
        _ = CloseAfterDelayAsync(_closeCts.Token);
    }

    private void HandleContentMouseEnter()
    {
        CancelClose();
        ParentSubContext?.CancelClose?.Invoke();
    }

    private void HandleContentMouseLeave()
    {
        _closeCts = new CancellationTokenSource();
        _ = CloseAfterDelayAsync(_closeCts.Token);
    }

    private async Task OpenAfterDelayAsync(CancellationToken ct)
    {
        try
        {
            await Task.Delay(HoverOpenDelayMs, ct);
            _open = true;
            _context.Open = true;
            await InvokeAsync(StateHasChanged);
        }
        catch (OperationCanceledException) { }
    }

    private async Task CloseAfterDelayAsync(CancellationToken ct)
    {
        try
        {
            await Task.Delay(HoverCloseDelayMs, ct);
            _open = false;
            _context.Open = false;
            await InvokeAsync(StateHasChanged);
        }
        catch (OperationCanceledException) { }
    }

    private void CancelOpen()
    {
        _openCts?.Cancel();
        _openCts?.Dispose();
        _openCts = null;
    }

    private void CancelClose()
    {
        _closeCts?.Cancel();
        _closeCts?.Dispose();
        _closeCts = null;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        CancelOpen();
        CancelClose();
    }
}
