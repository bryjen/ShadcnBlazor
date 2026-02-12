using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Shared;
using ShadcnBlazor.Shared.Attributes;
using ShadcnBlazor.Shared.Enums;

namespace ShadcnBlazor.Components.Radio;

[ComponentMetadata(Name = nameof(RadioGroup), Description = "", Dependencies = [nameof(Radio), nameof(RadioCard)])]
public partial class RadioGroup : ShadcnComponentBase
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string? Value { get; set; }

    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    [Parameter]
    public Size Size { get; set; } = Size.Md;

    [Parameter]
    public bool Disabled { get; set; }

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

