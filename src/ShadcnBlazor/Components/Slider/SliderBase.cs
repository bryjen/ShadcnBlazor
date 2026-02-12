using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Shared;
using ShadcnBlazor.Shared.Enums;

namespace ShadcnBlazor.Components.Slider;

public abstract class SliderBase : ShadcnComponentBase
{
    [Parameter]
    public double Min { get; set; } = 0;

    [Parameter]
    public double Max { get; set; } = 100;

    [Parameter]
    public double Step { get; set; } = 1;

    [Parameter]
    public Size Size { get; set; } = Size.Md;

    [Parameter]
    public bool Disabled { get; set; }

    [Parameter]
    public string AriaLabel { get; set; } = "Slider";

    protected int TrackHeightPx => Size switch
    {
        Size.Xs => 3,
        Size.Sm => 4,
        Size.Md => 6,
        Size.Lg => 8,
        _ => 4,
    };

    protected int ThumbSizePx => Size switch
    {
        Size.Xs => 10,
        Size.Sm => 12,
        Size.Md => 16,
        Size.Lg => 20,
        _ => 16,
    };

    protected string GetContainerClass()
    {
        var baseClasses = Size switch
        {
            Size.Xs => "relative w-full h-5 select-none",
            Size.Sm => "relative w-full h-5 select-none",
            Size.Md => "relative w-full h-6 select-none",
            Size.Lg => "relative w-full h-7 select-none",
            _ => "relative w-full h-6 select-none",
        };

        var stateClasses = Disabled ? "opacity-50" : string.Empty;
        return MergeCss(baseClasses, stateClasses, Class);
    }

    protected string GetInputClass()
    {
        var baseClasses = "absolute inset-0 h-full w-full cursor-pointer opacity-0 outline-none disabled:cursor-not-allowed";
        return MergeCss(baseClasses);
    }

    protected string GetTrackStyle() => $"height: {TrackHeightPx}px;";

    protected string GetRangeStyle(double startPercent, double endPercent)
    {
        var left = Math.Min(startPercent, endPercent);
        var width = Math.Abs(endPercent - startPercent);
        return $"height: {TrackHeightPx}px; left: {left:0.##}%; width: {width:0.##}%;";
    }

    protected string GetThumbStyle(double percent)
    {
        var halfThumb = ThumbSizePx / 2.0;
        return $"width: {ThumbSizePx}px; height: {ThumbSizePx}px; left: calc({percent:0.##}% - {halfThumb:0.##}px);";
    }

    protected double ClampValue(double value)
    {
        if (Max <= Min)
        {
            return Min;
        }

        return Math.Clamp(value, Min, Max);
    }

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

    protected bool TryParseDouble(ChangeEventArgs e, out double value)
    {
        return double.TryParse(e.Value?.ToString(), out value);
    }
}
