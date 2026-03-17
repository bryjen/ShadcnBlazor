using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Popover.Models;

namespace ShadcnBlazor.Components.Popover;

public partial class PopoverHostItem : ComponentBase
{
    [Parameter, EditorRequired]
    public required object RegistrationObject { get; set; }

    private PopoverRegistration Registration => (PopoverRegistration)RegistrationObject;
    private object? _lastRenderedRegistration;

    protected override bool ShouldRender()
    {
        if (_lastRenderedRegistration is null)
        {
            return true;
        }

        return !ReferenceEquals(_lastRenderedRegistration, RegistrationObject);
    }

    protected override void OnAfterRender(bool firstRender)
    {
        _lastRenderedRegistration = RegistrationObject;
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
