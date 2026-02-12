using Microsoft.AspNetCore.Components;

namespace ShadcnBlazor.Components.Popover;

public partial class PopoverProvider : ComponentBase, IDisposable
{
    private readonly Dictionary<string, PopoverRegistration> _registrations = new(StringComparer.Ordinal);
    private int? _appliedDebounceMilliseconds;

    [Inject]
    public required IPopoverRegistry PopoverRegistry { get; set; }

    [Inject]
    public required IPopoverService PopoverService { get; set; }

    [Parameter]
    public string Class { get; set; } = string.Empty;

    [Parameter]
    public int RepositionDebounceMilliseconds { get; set; } = 25;

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    protected override void OnInitialized()
    {
        PopoverRegistry.SetProvider(this);
    }

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

    public void Dispose()
    {
        PopoverRegistry.ClearProvider(this);
    }

    internal void RegisterOrUpdate(PopoverRegistration registration)
    {
        if (_registrations.TryGetValue(registration.PopoverId, out var existing))
        {
            if (AreEquivalent(existing, registration))
            {
                return;
            }
        }

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

    private static bool AreEquivalent(PopoverRegistration left, PopoverRegistration right)
    {
        return left.PopoverId == right.PopoverId
            && left.AnchorId == right.AnchorId
            && left.Content == right.Content
            && left.Open == right.Open
            && left.AnchorOrigin == right.AnchorOrigin
            && left.TransformOrigin == right.TransformOrigin
            && left.WidthMode == right.WidthMode
            && left.ClampList == right.ClampList
            && left.PopoverClass == right.PopoverClass
            && left.PopoverAttributes == right.PopoverAttributes;
    }

    private Dictionary<string, object> GetPopoverAttributes(PopoverRegistration registration)
    {
        if (registration.PopoverAttributes is null)
        {
            return new Dictionary<string, object>(StringComparer.Ordinal)
            {
                ["data-anchor-id"] = registration.AnchorId
            };
        }

        if (registration.PopoverAttributes.ContainsKey("data-anchor-id"))
        {
            return registration.PopoverAttributes;
        }

        var merged = new Dictionary<string, object>(registration.PopoverAttributes, StringComparer.Ordinal)
        {
            ["data-anchor-id"] = registration.AnchorId
        };

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
            "popover-open",
            ToTransformClass(registration.TransformOrigin),
            ToAnchorClass(registration.AnchorOrigin)
        };

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
}
