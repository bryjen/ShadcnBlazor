using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SampleWasmProject.Components.Core.Popover.Models;
using SampleWasmProject.Components.Core.Popover.Services;

namespace SampleWasmProject.Components.Core.Popover;

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
    /// Content rendered as the anchor/trigger element.
    /// </summary>
    [Parameter]
    public RenderFragment? Anchor { get; set; }

    /// <summary>
    /// Content rendered inside the popover panel.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Whether the popover is open.
    /// </summary>
    [Parameter]
    public bool Open { get; set; }

    /// <summary>
    /// Callback invoked when the open state changes.
    /// </summary>
    [Parameter]
    public EventCallback<bool> OpenChanged { get; set; }

    /// <summary>
    /// Whether clicking outside closes the popover.
    /// </summary>
    [Parameter]
    public bool CloseOnOutsideClick { get; set; }

    /// <summary>
    /// Whether to animate open/close transitions.
    /// </summary>
    [Parameter]
    public bool Animate { get; set; } = true;

    /// <summary>
    /// Duration of the close animation in milliseconds.
    /// </summary>
    [Parameter]
    public int ExitAnimationDurationMs { get; set; } = 140;

    /// <summary>
    /// Where the popover is anchored relative to the trigger.
    /// </summary>
    [Parameter]
    public PopoverPlacement AnchorOrigin { get; set; } = PopoverPlacement.BottomLeft;

    /// <summary>
    /// Transform origin for the popover content.
    /// </summary>
    [Parameter]
    public PopoverPlacement TransformOrigin { get; set; } = PopoverPlacement.TopLeft;

    /// <summary>
    /// How the popover width is determined.
    /// </summary>
    [Parameter]
    public PopoverWidthMode WidthMode { get; set; } = PopoverWidthMode.None;

    /// <summary>
    /// Whether to clamp the popover within the viewport.
    /// </summary>
    [Parameter]
    public bool ClampList { get; set; }

    /// <summary>
    /// CSS classes for the anchor element.
    /// </summary>
    [Parameter]
    public string AnchorClass { get; set; } = string.Empty;

    /// <summary>
    /// Additional attributes for the anchor element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AnchorAttributes { get; set; }

    /// <summary>
    /// CSS classes for the popover panel.
    /// </summary>
    [Parameter]
    public string PopoverClass { get; set; } = string.Empty;

    /// <summary>
    /// Additional attributes for the popover panel.
    /// </summary>
    [Parameter]
    public Dictionary<string, object>? PopoverAttributes { get; set; }

    /// <summary>
    /// Gap in pixels between the popover and its anchor. Applied in the direction away from the anchor (respects flip).
    /// </summary>
    [Parameter]
    public int Offset { get; set; }

    private string AnchorId => $"anchor-{_popoverId}";

    private string AnchorClassValue
    {
        get
        {
            if (string.IsNullOrWhiteSpace(AnchorClass))
            {
                return "popover-anchor";
            }

            return $"popover-anchor {AnchorClass}";
        }
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
                Offset = Offset
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
                await PopoverService.ConnectAsync(AnchorId, _popoverId);
            }
            else
            {
                await PopoverService.DisconnectAsync(_popoverId);
            }

            _lastConnectedOpen = _visualOpen;
        }

        await UpdateOutsideClickSubscriptionAsync();
    }

    /// <summary>
    /// Handles pointer down events outside the popover for close-on-outside-click behavior.
    /// </summary>
    [JSInvokable]
    public async Task HandleOutsidePointerDown()
    {
        if (!_visualOpen || !CloseOnOutsideClick)
        {
            return;
        }

        if (OpenChanged.HasDelegate)
        {
            await OpenChanged.InvokeAsync(false);
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

        await DisableOutsideClickSubscriptionAsync();
        _outsideClickReference?.Dispose();

        await PopoverService.DisconnectAsync(_popoverId);
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
}
