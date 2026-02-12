using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Shared;
using ShadcnBlazor.Shared.Enums;

namespace ShadcnBlazor.Components.Radio;

public abstract class RadioSelectableComponentBase : ShadcnComponentBase
{
    [CascadingParameter]
    internal RadioGroup? ParentGroup { get; set; }

    [Parameter]
    public bool Checked { get; set; }

    [Parameter]
    public EventCallback<bool> CheckedChanged { get; set; }

    [Parameter]
    public string? Value { get; set; }

    [Parameter]
    public Size Size { get; set; } = Size.Md;

    [Parameter]
    public bool Disabled { get; set; }

    protected bool IsInGroup => ParentGroup is not null;

    protected bool IsChecked => IsInGroup
        ? Value is not null && string.Equals(ParentGroup!.Value, Value, StringComparison.Ordinal)
        : Checked;

    protected bool IsDisabled => Disabled || (ParentGroup?.Disabled ?? false);

    protected Size EffectiveSize => ParentGroup?.Size ?? Size;

    protected async Task SelectAsync()
    {
        if (IsDisabled)
        {
            return;
        }

        if (IsInGroup)
        {
            if (Value is not null)
            {
                await ParentGroup!.SetValueAsync(Value);
            }
            return;
        }

        if (!Checked)
        {
            Checked = true;
            await CheckedChanged.InvokeAsync(Checked);
        }
    }
}
