using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Popover.Models;
using ShadcnBlazor.Components.Popover.Services;

namespace ShadcnBlazor.Components.Popover;

/// <summary>
/// Provider component that hosts popover content and manages positioning. Required for Popover to work.
/// </summary>
public partial class PopoverProvider : ComponentBase, IDisposable
{
    private readonly Dictionary<string, PopoverRegistration> _registrations = new(StringComparer.Ordinal);
    private int? _appliedDebounceMilliseconds;

    /// <summary>
    /// Injected registry for popover coordination.
    /// </summary>
    [Inject]
    public required IPopoverRegistry PopoverRegistry { get; set; }

    /// <summary>
    /// Injected popover service for JavaScript interop.
    /// </summary>
    [Inject]
    public required IPopoverService PopoverService { get; set; }

    /// <summary>
    /// CSS classes for the provider root element.
    /// </summary>
    [Parameter]
    public string Class { get; set; } = string.Empty;

    /// <summary>
    /// Debounce delay in milliseconds when repositioning popovers.
    /// </summary>
    [Parameter]
    public int RepositionDebounceMilliseconds { get; set; } = 25;

    /// <summary>
    /// Additional HTML attributes for the provider root.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        PopoverRegistry.SetProvider(this);
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var normalizedDebounce = Math.Max(0, RepositionDebounceMilliseconds);
        if (_appliedDebounceMilliseconds == normalizedDebounce)
        {
            return;
        }

        await PopoverService.SetRepositionDebounceAsync(normalizedDebounce);
        _appliedDebounceMilliseconds = normalizedDebounce;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        PopoverRegistry.ClearProvider(this);
    }

    internal void RegisterOrUpdate(PopoverRegistration registration)
    {
        _registrations[registration.PopoverId] = registration;
        StateHasChanged();
    }

    internal void Unregister(string popoverId)
    {
        if (_registrations.Remove(popoverId))
        {
            StateHasChanged();
        }
    }

    private string RootClass
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Class))
            {
                return "popover-provider";
            }

            return $"popover-provider {Class}";
        }
    }
}
