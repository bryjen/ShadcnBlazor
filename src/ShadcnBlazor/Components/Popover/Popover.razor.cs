using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShadcnBlazor.Components.Popover.Models;
using ShadcnBlazor.Components.Popover.Services;
using ShadcnBlazor.Components.Shared;
using ShadcnBlazor.Components.Shared.Models.Accessibility;
using ShadcnBlazor.Components.Shared.Services;
using TailwindMerge;

namespace ShadcnBlazor.Components.Popover;

/// <summary>
/// Floating panel anchored to a trigger element; requires PopoverProvider in layout.
/// </summary>
public partial class Popover : ComponentBase, IAsyncDisposable
{
    private readonly string _popoverId = $"popover-{Guid.NewGuid():N}";
    private bool _isRegistered;
    private bool _renderPopover;
    private bool _visualOpen;
    private bool? _lastConnectedOpen;
    private bool _isClosing;
    private bool _scrollLocked;
    private PopoverProvider? _registeredProvider;
    private DotNetObjectReference<Popover>? _outsideClickReference;
    private bool _outsideClickSubscriptionActive;
    private CancellationTokenSource? _closeAnimationCts;

    /// <summary>
    /// Injected popover service for JavaScript interop.
    /// </summary>
    [Inject]
    public required IPopoverService PopoverService { get; set; }

    /// <summary>
    /// Injected registry for popover provider coordination.
    /// </summary>
    [Inject]
    public required IPopoverRegistry PopoverRegistry { get; set; }

    /// <summary>
    /// Injected scroll lock service for optional body scroll lock.
    /// </summary>
    [Inject]
    public required ScrollLockService ScrollLock { get; set; }

    /// <summary>
    /// Content rendered as the anchor/trigger element.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Content)]
    public RenderFragment? Anchor { get; set; }

    /// <summary>
    /// Content rendered inside the popover panel.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Content)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Whether the popover is open.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public bool Open { get; set; }

    /// <summary>
    /// Callback invoked when the open state changes.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public EventCallback<bool> OpenChanged { get; set; }

    /// <summary>
    /// Whether clicking outside closes the popover.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public bool CloseOnOutsideClick { get; set; }

    /// <summary>
    /// Whether to lock body scroll while the popover is open.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public bool LockScroll { get; set; }

    /// <summary>
    /// Whether to animate open/close transitions.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public bool Animate { get; set; } = true;

    /// <summary>
    /// Duration of the close animation in milliseconds.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public int ExitAnimationDurationMs { get; set; } = 140;

    /// <summary>
    /// Where the popover is anchored relative to the trigger.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Appearance)]
    public PopoverPlacement AnchorOrigin { get; set; } = PopoverPlacement.BottomLeft;

    /// <summary>
    /// Transform origin for the popover content.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Appearance)]
    public PopoverPlacement TransformOrigin { get; set; } = PopoverPlacement.TopLeft;

    /// <summary>
    /// How the popover width is determined.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Appearance)]
    public PopoverWidthMode WidthMode { get; set; } = PopoverWidthMode.None;

    /// <summary>
    /// Whether to clamp the popover within the viewport.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public bool ClampList { get; set; }

    /// <summary>
    /// CSS classes for the anchor element.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Appearance)]
    public string AnchorClass { get; set; } = string.Empty;

    /// <summary>
    /// Additional attributes for the anchor element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    [Category(ComponentCategory.Common)]
    public Dictionary<string, object>? AnchorAttributes { get; set; }

    /// <summary>
    /// CSS classes for the popover panel.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Appearance)]
    public string PopoverClass { get; set; } = string.Empty;

    /// <summary>
    /// Additional attributes for the popover panel.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Common)]
    public Dictionary<string, object>? PopoverAttributes { get; set; }

    /// <summary>
    /// Gap in pixels between the popover and its anchor. Applied in the direction away from the anchor (respects flip).
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Appearance)]
    public int Offset { get; set; }

    /// <summary>
    /// A secondary offset along the alignment axis, independent of sideOffset.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Appearance)]
    public int AlignOffset { get; set; }

    /// <summary>
    /// Hide the popover when the anchor element is scrolled completely out of the viewport.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public bool HideWhenDetached { get; set; }

    /// <summary>
    /// Callback invoked when interaction occurs outside the popover.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public EventCallback<InteractOutsideEventArgs> OnInteractOutside { get; set; }

    /// <summary>
    /// Value for aria-haspopup on the trigger. Use "menu" for dropdown menus, "listbox" for selects, "dialog" for dialogs.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Common)]
    public string AriaHasPopup { get; set; } = "dialog";

    /// <summary>
    /// Injected TailwindMerge service for merging Tailwind CSS classes.
    /// </summary>
    [Inject]
    public required TwMerge TwMerge { get; set; }

    private string AnchorId => $"anchor-{_popoverId}";

    private PopoverTriggerContext _popoverTriggerContext => new()
    {
        Open = _visualOpen,
        PopoverId = _popoverId,
        AriaHasPopup = AriaHasPopup
    };

    private string AnchorClassValue
    {
        get
        {
            if (string.IsNullOrWhiteSpace(AnchorClass))
            {
                return "popover-anchor inline-flex w-fit max-w-max";
            }

            return MergeCss("popover-anchor inline-flex w-fit max-w-max", AnchorClass);
        }
    }

    private string MergeCss(params string[] classes)
    {
        var joined = string.Join(" ", classes);
        return TwMerge.Merge(joined) ?? joined;
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        var provider = PopoverRegistry.CurrentProvider;
        if (provider is null)
        {
            return;
        }

        if (_registeredProvider is not null && !ReferenceEquals(_registeredProvider, provider))
        {
            _registeredProvider.Unregister(_popoverId);
            _isRegistered = false;
        }

        if (Open)
        {
            CancelCloseAnimation();
            _renderPopover = true;
            _visualOpen = true;
            _isClosing = false;
        }
        else if (_renderPopover)
        {
            if (!Animate)
            {
                CancelCloseAnimation();
                _renderPopover = false;
                _visualOpen = false;
                _isClosing = false;
            }
            else if (!_isClosing)
            {
                _visualOpen = false;
                _isClosing = true;
                StartCloseAnimation();
            }
        }
        else
        {
            _visualOpen = false;
            _isClosing = false;
        }

        _registeredProvider = provider;

        if (_renderPopover)
        {
            provider.RegisterOrUpdate(new PopoverRegistration
            {
                PopoverId = _popoverId,
                AnchorId = AnchorId,
                Content = ChildContent,
                Render = true,
                Open = _visualOpen,
                AnchorOrigin = AnchorOrigin,
                TransformOrigin = TransformOrigin,
                WidthMode = WidthMode,
                ClampList = ClampList,
                PopoverClass = PopoverClass,
                PopoverAttributes = PopoverAttributes,
                Offset = Offset,
                AlignOffset = AlignOffset,
                HideWhenDetached = HideWhenDetached
            });

            _isRegistered = true;
        }
        else if (_isRegistered)
        {
            _registeredProvider.Unregister(_popoverId);
            _isRegistered = false;
        }
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (PopoverRegistry.CurrentProvider is null)
        {
            return;
        }

        if (!_lastConnectedOpen.HasValue || _lastConnectedOpen.Value != _visualOpen || firstRender)
        {
            if (_visualOpen)
            {
                var options = new
                {
                    placement = PopoverHostItem.ToFloatingPlacement(TransformOrigin),
                    anchorPlacement = PopoverHostItem.ToFloatingPlacement(AnchorOrigin),
                    widthMode = WidthMode.ToString().ToLowerInvariant(),
                    clampList = ClampList,
                    offset = Offset,
                    alignOffset = AlignOffset,
                    hideWhenDetached = HideWhenDetached
                };
                await PopoverService.ConnectAsync(AnchorId, _popoverId, options);
            }
            else
            {
                await PopoverService.DisconnectAsync(_popoverId);
            }

            _lastConnectedOpen = _visualOpen;
        }

        await UpdateOutsideClickSubscriptionAsync();
        await UpdateScrollLockAsync();
    }

    /// <summary>
    /// Handles interaction events outside the popover (click or focus).
    /// </summary>
    [JSInvokable]
    public async Task HandleInteractOutside(InteractOutsideEventType eventType)
    {
        if (!_visualOpen)
        {
            return;
        }

        var args = new InteractOutsideEventArgs { EventType = eventType };
        if (OnInteractOutside.HasDelegate)
        {
            await OnInteractOutside.InvokeAsync(args);
        }

        if (args.Handled)
        {
            return;
        }

        if (CloseOnOutsideClick)
        {
            if (OpenChanged.HasDelegate)
            {
                await OpenChanged.InvokeAsync(false);
            }
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        CancelCloseAnimation();

        if (_isRegistered)
        {
            _registeredProvider?.Unregister(_popoverId);
        }

        try
        {
            await DisableOutsideClickSubscriptionAsync();
            _outsideClickReference?.Dispose();

            await PopoverService.DisconnectAsync(_popoverId);
            if (_scrollLocked)
            {
                await ScrollLock.UnlockAsync();
                _scrollLocked = false;
            }
        }
        catch (JSDisconnectedException) { }
        catch (TaskCanceledException) { }
    }

    private void StartCloseAnimation()
    {
        CancelCloseAnimation();

        var normalizedDuration = Math.Max(0, ExitAnimationDurationMs);
        if (normalizedDuration == 0)
        {
            _renderPopover = false;
            _isClosing = false;
            return;
        }

        _closeAnimationCts = new CancellationTokenSource();
        _ = FinalizeCloseAfterDelayAsync(normalizedDuration, _closeAnimationCts.Token);
    }

    private async Task FinalizeCloseAfterDelayAsync(int delayMs, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(delayMs, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        if (cancellationToken.IsCancellationRequested || Open)
        {
            return;
        }

        _renderPopover = false;
        _isClosing = false;
        await InvokeAsync(StateHasChanged);
    }

    private void CancelCloseAnimation()
    {
        if (_closeAnimationCts is null)
        {
            return;
        }

        _closeAnimationCts.Cancel();
        _closeAnimationCts.Dispose();
        _closeAnimationCts = null;
    }

    private async Task UpdateOutsideClickSubscriptionAsync()
    {
        if (_visualOpen && CloseOnOutsideClick)
        {
            if (_outsideClickReference is null)
            {
                _outsideClickReference = DotNetObjectReference.Create(this);
            }

            if (!_outsideClickSubscriptionActive)
            {
                await PopoverService.EnableOutsideClickCloseAsync(AnchorId, _popoverId, _outsideClickReference);
                _outsideClickSubscriptionActive = true;
            }

            return;
        }

        await DisableOutsideClickSubscriptionAsync();
    }

    private async Task DisableOutsideClickSubscriptionAsync()
    {
        if (!_outsideClickSubscriptionActive)
        {
            return;
        }

        await PopoverService.DisableOutsideClickCloseAsync(_popoverId);
        _outsideClickSubscriptionActive = false;
    }

    private async Task UpdateScrollLockAsync()
    {
        var shouldLock = LockScroll && _visualOpen;
        if (shouldLock && !_scrollLocked)
        {
            await ScrollLock.LockAsync();
            _scrollLocked = true;
        }
        else if (!shouldLock && _scrollLocked)
        {
            await ScrollLock.UnlockAsync();
            _scrollLocked = false;
        }
    }
}
