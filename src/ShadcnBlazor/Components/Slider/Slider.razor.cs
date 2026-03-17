using Microsoft.AspNetCore.Components;

namespace ShadcnBlazor.Components.Slider;

/// <summary>
/// Single-thumb slider for selecting a value within a min/max range.
/// </summary>
public partial class Slider : SliderBase
{
    /// <summary>
    /// The current value.
    /// </summary>
    [Parameter]
    public double Value { get; set; }

    /// <summary>
    /// Callback invoked when the value changes.
    /// </summary>
    [Parameter]
    public EventCallback<double> ValueChanged { get; set; }

    /// <summary>
    /// Callback invoked when the user finishes changing the value (on change event).
    /// </summary>
    [Parameter]
    public EventCallback<double> OnChange { get; set; }

    private double ValuePercent => ToPercent(Value);

    private string GetCurrentRangeStyle() => GetRangeStyle(0, ValuePercent);

    private string GetCurrentThumbStyle() => GetThumbStyle(ValuePercent);

    private async Task HandleInput(ChangeEventArgs e)
    {
        if (!TryParseDouble(e, out var newValue))
        {
            return;
        }

        Value = ClampValue(newValue);
        await ValueChanged.InvokeAsync(Value);
    }

    private async Task HandleChange(ChangeEventArgs e)
    {
        if (!TryParseDouble(e, out var newValue))
        {
            return;
        }

        await OnChange.InvokeAsync(ClampValue(newValue));
    }
}
