using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Shared;
using ShadcnBlazor.Shared.Enums;

namespace ShadcnBlazor.Components.Slider;

/// <summary>
/// Base class for Slider and RangeSlider, providing track, thumb, and value logic.
/// </summary>
public abstract class SliderBase : ShadcnComponentBase
{
    /// <summary>
    /// Minimum value of the slider.
    /// </summary>
    [Parameter]
    public double Min { get; set; } = 0;

    /// <summary>
    /// Maximum value of the slider.
    /// </summary>
    [Parameter]
    public double Max { get; set; } = 100;

    /// <summary>
    /// Step increment for the value.
    /// </summary>
    [Parameter]
    public double Step { get; set; } = 1;

    /// <summary>
    /// Size of the slider (affects track and thumb dimensions).
    /// </summary>
    [Parameter]
    public Size Size { get; set; } = Size.Md;

    /// <summary>
    /// Whether the slider is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// ARIA label for accessibility.
    /// </summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Slider";

    /// <summary>
    /// Track height in pixels based on size.
    /// </summary>
    protected int TrackHeightPx => Size switch
    {
        Size.Sm => 2,
        Size.Md => 3,
        Size.Lg => 5,
        _ => 3,
    };

    /// <summary>
    /// Thumb size in pixels based on size.
    /// </summary>
    protected int ThumbSizePx => Size switch
    {
        Size.Sm => 9,
        Size.Md => 11,
        Size.Lg => 15,
        _ => 11,
    };

    /// <summary>
    /// Returns CSS classes for the slider container.
    /// </summary>
    protected string GetContainerClass()
    {
        var baseClasses = Size switch
        {
            Size.Sm => "relative w-full h-4 select-none",
            Size.Md => "relative w-full h-5 select-none",
            Size.Lg => "relative w-full h-6 select-none",
            _ => "relative w-full h-5 select-none",
        };

        var stateClasses = Disabled ? "opacity-50" : string.Empty;
        return MergeCss(baseClasses, stateClasses, Class);
    }

    /// <summary>
    /// Returns CSS classes for the hidden input element.
    /// </summary>
    protected string GetInputClass()
    {
        var baseClasses = "absolute inset-0 h-full w-full cursor-pointer opacity-0 outline-none disabled:cursor-not-allowed";
        return MergeCss(baseClasses);
    }

    /// <summary>
    /// Returns inline style for the track.
    /// </summary>
    protected string GetTrackStyle() => $"height: {TrackHeightPx}px;";

    /// <summary>
    /// Returns inline style for the filled range segment.
    /// </summary>
    protected string GetRangeStyle(double startPercent, double endPercent)
    {
        var left = Math.Min(startPercent, endPercent);
        var width = Math.Abs(endPercent - startPercent);
        return $"height: {TrackHeightPx}px; left: {left:0.##}%; width: {width:0.##}%;";
    }

    /// <summary>
    /// Returns inline style for the thumb at the given percent position.
    /// </summary>
    protected string GetThumbStyle(double percent)
    {
        var halfThumb = ThumbSizePx / 2.0;
        return $"width: {ThumbSizePx}px; height: {ThumbSizePx}px; left: calc({percent:0.##}% - {halfThumb:0.##}px);";
    }

    /// <summary>
    /// Clamps a value to the Min/Max range.
    /// </summary>
    protected double ClampValue(double value)
    {
        if (Max <= Min)
        {
            return Min;
        }

        return Math.Clamp(value, Min, Max);
    }

    /// <summary>
    /// Converts a value to a percentage (0-100) within the Min/Max range.
    /// </summary>
    protected double ToPercent(double value)
    {
        var range = Max - Min;
        if (range <= 0)
        {
            return 0;
        }

        var clamped = ClampValue(value);
        return ((clamped - Min) / range) * 100;
    }

    /// <summary>
    /// Tries to parse the change event value as a double.
    /// </summary>
    protected bool TryParseDouble(ChangeEventArgs e, out double value)
    {
        return double.TryParse(e.Value?.ToString(), out value);
    }
}
