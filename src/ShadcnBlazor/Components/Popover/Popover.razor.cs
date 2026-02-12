using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Shared.Attributes;

namespace ShadcnBlazor.Components.Popover;

[ComponentMetadata(Name = nameof(Popover), Description = "", Dependencies = [])]
public partial class Popover : ComponentBase, IAsyncDisposable
{
    private readonly string _popoverId = $"popover-{Guid.NewGuid():N}";
    private bool _isRegistered;
    private bool? _lastOpen;
    private PopoverProvider? _registeredProvider;

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

        provider.RegisterOrUpdate(new PopoverRegistration
        {
            PopoverId = _popoverId,
            AnchorId = AnchorId,
            Content = ChildContent,
            Open = Open,
            AnchorOrigin = AnchorOrigin,
            TransformOrigin = TransformOrigin,
            WidthMode = WidthMode,
            ClampList = ClampList,
            PopoverClass = PopoverClass,
            PopoverAttributes = PopoverAttributes
        });

        _registeredProvider = provider;
        _isRegistered = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (PopoverRegistry.CurrentProvider is null)
        {
            return;
        }

        if (!_lastOpen.HasValue || _lastOpen.Value != Open || firstRender)
        {
            if (Open)
            {
                await PopoverService.ConnectAsync(AnchorId, _popoverId);
            }
            else
            {
                await PopoverService.DisconnectAsync(_popoverId);
            }

            _lastOpen = Open;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_isRegistered)
        {
            _registeredProvider?.Unregister(_popoverId);
        }

        await PopoverService.DisconnectAsync(_popoverId);
    }
}
