using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;
using ShadcnBlazor.Components.Shared.Enums;

namespace ShadcnBlazor.Components.Radio;

/// <summary>
/// Container for Radio or RadioCard options, managing single-selection state.
/// </summary>
public partial class RadioGroup : ShadcnComponentBase
{
    /// <summary>
    /// The Radio or RadioCard options.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The selected value (Value of the selected Radio/RadioCard).
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Callback invoked when the selected value changes.
    /// </summary>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// Size applied to all child radios.
    /// </summary>
    [Parameter]
    public Size Size { get; set; } = Size.Md;

    /// <summary>
    /// Whether all options in the group are disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Whether to lay out options vertically (true) or horizontally (false).
    /// </summary>
    [Parameter]
    public bool Vertical { get; set; } = true;

    internal async Task SetValueAsync(string? value)
    {
        if (Disabled)
        {
            return;
        }

        if (string.Equals(Value, value, StringComparison.Ordinal))
        {
            return;
        }

        Value = value;
        await ValueChanged.InvokeAsync(Value);
        StateHasChanged();
    }

    private string GetClass()
    {
        var direction = Vertical ? "flex flex-col gap-2" : "flex items-center gap-4";
        return MergeCss(direction, Class);
    }
}

