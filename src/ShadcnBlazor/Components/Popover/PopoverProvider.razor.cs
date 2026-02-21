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


    private Dictionary<string, object> GetPopoverAttributes(PopoverRegistration registration)
    {
        var merged = registration.PopoverAttributes is null
            ? new Dictionary<string, object>(StringComparer.Ordinal)
            : new Dictionary<string, object>(registration.PopoverAttributes, StringComparer.Ordinal);

        merged["data-anchor-id"] = registration.AnchorId;
        merged["data-state"] = registration.Open ? "open" : "closed";
        merged["data-side"] = ToSide(registration.AnchorOrigin);
        merged["data-resolved-side"] = ToSide(registration.AnchorOrigin);
        merged["aria-hidden"] = registration.Open ? "false" : "true";
        if (registration.Offset > 0)
        {
            merged["data-offset"] = registration.Offset.ToString();
        }

        return merged;
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

    private string GetPopoverClass(PopoverRegistration registration)
    {
        var classes = new List<string>
        {
            "popover-content",
            ToTransformClass(registration.TransformOrigin),
            ToAnchorClass(registration.AnchorOrigin)
        };

        if (registration.Open)
        {
            classes.Add("popover-open");
        }

        switch (registration.WidthMode)
        {
            case PopoverWidthMode.Relative:
                classes.Add("popover-relative-width");
                break;
            case PopoverWidthMode.Adaptive:
                classes.Add("popover-adaptive-width");
                break;
        }

        if (!string.IsNullOrWhiteSpace(registration.PopoverClass))
        {
            classes.Add(registration.PopoverClass);
        }

        return string.Join(" ", classes);
    }

    private static string ToTransformClass(PopoverPlacement placement)
    {
        return placement switch
        {
            PopoverPlacement.TopLeft => "popover-top-left",
            PopoverPlacement.TopCenter => "popover-top-center",
            PopoverPlacement.TopRight => "popover-top-right",
            PopoverPlacement.CenterLeft => "popover-center-left",
            PopoverPlacement.Center => "popover-center-center",
            PopoverPlacement.CenterRight => "popover-center-right",
            PopoverPlacement.BottomLeft => "popover-bottom-left",
            PopoverPlacement.BottomCenter => "popover-bottom-center",
            PopoverPlacement.BottomRight => "popover-bottom-right",
            _ => "popover-bottom-left"
        };
    }

    private static string ToAnchorClass(PopoverPlacement placement)
    {
        return placement switch
        {
            PopoverPlacement.TopLeft => "popover-anchor-top-left",
            PopoverPlacement.TopCenter => "popover-anchor-top-center",
            PopoverPlacement.TopRight => "popover-anchor-top-right",
            PopoverPlacement.CenterLeft => "popover-anchor-center-left",
            PopoverPlacement.Center => "popover-anchor-center-center",
            PopoverPlacement.CenterRight => "popover-anchor-center-right",
            PopoverPlacement.BottomLeft => "popover-anchor-bottom-left",
            PopoverPlacement.BottomCenter => "popover-anchor-bottom-center",
            PopoverPlacement.BottomRight => "popover-anchor-bottom-right",
            _ => "popover-anchor-bottom-left"
        };
    }

    private static string ToSide(PopoverPlacement placement)
    {
        return placement switch
        {
            PopoverPlacement.TopLeft => "top",
            PopoverPlacement.TopCenter => "top",
            PopoverPlacement.TopRight => "top",
            PopoverPlacement.BottomLeft => "bottom",
            PopoverPlacement.BottomCenter => "bottom",
            PopoverPlacement.BottomRight => "bottom",
            PopoverPlacement.CenterLeft => "left",
            PopoverPlacement.CenterRight => "right",
            _ => "bottom"
        };
    }
}
