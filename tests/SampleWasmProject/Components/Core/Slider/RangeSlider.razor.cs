using Microsoft.AspNetCore.Components;

namespace SampleWasmProject.Components.Core.Slider;

/// <summary>
/// Dual-thumb range slider for selecting a min and max value within a range.
/// </summary>
public partial class RangeSlider : SliderBase
{
    /// <summary>
    /// The lower (min) value of the range.
    /// </summary>
    [Parameter]
    public double LowerValue { get; set; } = 25;

    /// <summary>
    /// Callback invoked when the lower value changes.
    /// </summary>
    [Parameter]
    public EventCallback<double> LowerValueChanged { get; set; }

    /// <summary>
    /// The upper (max) value of the range.
    /// </summary>
    [Parameter]
    public double UpperValue { get; set; } = 75;

    /// <summary>
    /// Callback invoked when the upper value changes.
    /// </summary>
    [Parameter]
    public EventCallback<double> UpperValueChanged { get; set; }

    /// <summary>
    /// Callback invoked when either value changes (provides both lower and upper).
    /// </summary>
    [Parameter]
    public EventCallback<(double Lower, double Upper)> OnChange { get; set; }

    /// <summary>
    /// ARIA label for the lower thumb.
    /// </summary>
    [Parameter]
    public string LowerAriaLabel { get; set; } = "Lower value";

    /// <summary>
    /// ARIA label for the upper thumb.
    /// </summary>
    [Parameter]
    public string UpperAriaLabel { get; set; } = "Upper value";

    private double LowerPercent => ToPercent(LowerValue);
    private double UpperPercent => ToPercent(UpperValue);

    private string GetSelectedRangeStyle() => GetRangeStyle(LowerPercent, UpperPercent);

    // Split hit-test regions so each thumb can be picked independently.
    // Using left/right bounds is more reliable than clip-path for range inputs.
    private string GetLowerInputStyle()
    {
        var rightBound = 100 - UpperPercent;
        return $"right: {rightBound:0.##}%; z-index: 20;";
    }

    private string GetUpperInputStyle()
    {
        return $"left: {LowerPercent:0.##}%; z-index: 30;";
    }

    private string GetLowerThumbStyle() => GetThumbStyle(LowerPercent);

    private string GetUpperThumbStyle() => GetThumbStyle(UpperPercent);

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        var low = ClampValue(LowerValue);
        var high = ClampValue(UpperValue);

        if (low > high)
        {
            low = high;
        }

        LowerValue = low;
        UpperValue = high;
    }

    private async Task HandleLowerInput(ChangeEventArgs e)
    {
        if (!TryParseDouble(e, out var newValue))
        {
            return;
        }

        LowerValue = Math.Min(ClampValue(newValue), UpperValue);
        await LowerValueChanged.InvokeAsync(LowerValue);
    }

    private async Task HandleUpperInput(ChangeEventArgs e)
    {
        if (!TryParseDouble(e, out var newValue))
        {
            return;
        }

        UpperValue = Math.Max(ClampValue(newValue), LowerValue);
        await UpperValueChanged.InvokeAsync(UpperValue);
    }

    private async Task HandleLowerChange(ChangeEventArgs e)
    {
        if (!TryParseDouble(e, out var newValue))
        {
            return;
        }

        var lower = Math.Min(ClampValue(newValue), UpperValue);
        await OnChange.InvokeAsync((lower, UpperValue));
    }

    private async Task HandleUpperChange(ChangeEventArgs e)
    {
        if (!TryParseDouble(e, out var newValue))
        {
            return;
        }

        var upper = Math.Max(ClampValue(newValue), LowerValue);
        await OnChange.InvokeAsync((LowerValue, upper));
    }
}
