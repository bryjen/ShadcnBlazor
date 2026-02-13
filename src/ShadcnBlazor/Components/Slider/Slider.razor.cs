using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Shared.Attributes;

namespace ShadcnBlazor.Components.Slider;

[ComponentMetadata(Name = nameof(Slider), Description = "Single-thumb slider for selecting a value within a min/max range.", Dependencies = [])]
public partial class Slider : SliderBase
{
    [Parameter]
    public double Value { get; set; }

    [Parameter]
    public EventCallback<double> ValueChanged { get; set; }

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
