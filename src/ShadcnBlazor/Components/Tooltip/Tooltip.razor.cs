using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Popover;
using ShadcnBlazor.Components.Popover.Models;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Components.Tooltip;

/// <summary>
/// Hover-triggered tooltip that displays content above the trigger element.
/// </summary>
public partial class Tooltip
{
    /// <summary>
    /// Content rendered as the trigger element (e.g., icon or button).
    /// </summary>
    [Parameter]
    [Category("Content")]
    public RenderFragment? Anchor { get; set; }

    /// <summary>
    /// The tooltip content (supports multiline text).
    /// </summary>
    [Parameter]
    [Category("Content")]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Simple text content for the tooltip (alternative to ChildContent).
    /// </summary>
    [Parameter]
    [Category("Content")]
    public string? Content { get; set; }

    /// <summary>
    /// Delay in milliseconds before the tooltip appears on hover.
    /// </summary>
    [Parameter]
    [Category("Behavior")]
    public int DelayBeforeShowMs { get; set; } = 200;

    /// <summary>
    /// Delay in milliseconds before the tooltip hides after mouse leave.
    /// </summary>
    [Parameter]
    [Category("Behavior")]
    public int DelayBeforeHideMs { get; set; } = 50;

    /// <summary>
    /// Additional CSS classes for the tooltip content.
    /// </summary>
    [Parameter]
    [Category("Appearance")]
    public string ContentClass { get; set; } = string.Empty;

    /// <summary>
    /// Gap in pixels between the tooltip and its trigger.
    /// </summary>
    [Parameter]
    [Category("Appearance")]
    public int Offset { get; set; } = 8;

    private bool _open;
    private CancellationTokenSource? _showCts;
    private CancellationTokenSource? _hideCts;

    private async Task HandleMouseEnter()
    {
        CancelHide();
        _showCts = new CancellationTokenSource();
        try
        {
            await Task.Delay(DelayBeforeShowMs, _showCts.Token);
            _open = true;
            await InvokeAsync(StateHasChanged);
        }
        catch (OperationCanceledException) { }
    }

    private async Task HandleMouseLeave()
    {
        CancelShow();
        _hideCts = new CancellationTokenSource();
        try
        {
            await Task.Delay(DelayBeforeHideMs, _hideCts.Token);
            _open = false;
            await InvokeAsync(StateHasChanged);
        }
        catch (OperationCanceledException) { }
    }

    private void CancelShow()
    {
        _showCts?.Cancel();
        _showCts?.Dispose();
        _showCts = null;
    }

    private void CancelHide()
    {
        _hideCts?.Cancel();
        _hideCts?.Dispose();
        _hideCts = null;
    }

    public void Dispose()
    {
        CancelShow();
        CancelHide();
    }
}
