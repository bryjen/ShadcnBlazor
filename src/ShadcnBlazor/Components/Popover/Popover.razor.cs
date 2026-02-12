using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShadcnBlazor.Components.Popover.Models;
using ShadcnBlazor.Components.Popover.Services;
using ShadcnBlazor.Shared.Attributes;

namespace ShadcnBlazor.Components.Popover;

[ComponentMetadata(Name = nameof(Popover), Description = "", Dependencies = [])]
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

    [Inject]
    public required IPopoverService PopoverService { get; set; }

    [Inject]
    public required IPopoverRegistry PopoverRegistry { get; set; }

    [Parameter]
    public RenderFragment? Anchor { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public bool Open { get; set; }

    [Parameter]
    public EventCallback<bool> OpenChanged { get; set; }

    [Parameter]
    public bool CloseOnOutsideClick { get; set; }

    [Parameter]
    public bool Animate { get; set; } = true;

    [Parameter]
    public int ExitAnimationDurationMs { get; set; } = 140;

    [Parameter]
    public PopoverPlacement AnchorOrigin { get; set; } = PopoverPlacement.BottomLeft;

    [Parameter]
    public PopoverPlacement TransformOrigin { get; set; } = PopoverPlacement.TopLeft;

    [Parameter]
    public PopoverWidthMode WidthMode { get; set; } = PopoverWidthMode.None;

    [Parameter]
    public bool ClampList { get; set; }

    [Parameter]
    public string AnchorClass { get; set; } = string.Empty;

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AnchorAttributes { get; set; }

    [Parameter]
    public string PopoverClass { get; set; } = string.Empty;

    [Parameter]
    public Dictionary<string, object>? PopoverAttributes { get; set; }

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
                PopoverAttributes = PopoverAttributes
            });

            _isRegistered = true;
        }
        else if (_isRegistered)
        {
            _registeredProvider.Unregister(_popoverId);
            _isRegistered = false;
        }
    }

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
