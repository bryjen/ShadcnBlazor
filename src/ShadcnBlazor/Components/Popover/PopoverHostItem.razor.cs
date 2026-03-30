using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Popover.Models;

namespace ShadcnBlazor.Components.Popover;

/// <summary>Renders a single popover registration entry.</summary>
public partial class PopoverHostItem : ComponentBase
{
    /// <summary>The registration object for the popover.</summary>
    [Parameter, EditorRequired]
    public required object RegistrationObject { get; set; }

    private PopoverRegistration Registration => (PopoverRegistration)RegistrationObject;
    private object? _lastRenderedRegistration;

    /// <inheritdoc />
    protected override bool ShouldRender()
    {
        if (_lastRenderedRegistration is null) return true;
        return !ReferenceEquals(_lastRenderedRegistration, RegistrationObject);
    }

    /// <inheritdoc />
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
        var classes = new List<string> { "popover-content" };

        if (registration.Open)
            classes.Add("popover-open");

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
            classes.Add(registration.PopoverClass);

        return string.Join(" ", classes);
    }

    /// <summary>Maps placement to the cardinal side the popover appears on (used for CSS animations).</summary>
    internal static string ToSide(PopoverPlacement placement) => placement switch
    {
        PopoverPlacement.TopLeft or PopoverPlacement.TopCenter or PopoverPlacement.TopRight => "top",
        PopoverPlacement.BottomLeft or PopoverPlacement.BottomCenter or PopoverPlacement.BottomRight => "bottom",
        PopoverPlacement.CenterLeft => "left",
        PopoverPlacement.CenterRight => "right",
        _ => "bottom"
    };

    /// <summary>Maps placement to a Floating UI placement string.</summary>
    internal static string ToFloatingPlacement(PopoverPlacement placement) => placement switch
    {
        PopoverPlacement.TopLeft => "top-start",
        PopoverPlacement.TopCenter => "top",
        PopoverPlacement.TopRight => "top-end",
        PopoverPlacement.BottomLeft => "bottom-start",
        PopoverPlacement.BottomCenter => "bottom",
        PopoverPlacement.BottomRight => "bottom-end",
        PopoverPlacement.CenterLeft => "left",
        PopoverPlacement.CenterRight => "right",
        _ => "bottom"
    };
}
