using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;
using ShadcnBlazor.Components.Shared.Models.Enums;

namespace ShadcnBlazor.Components.Radio;

/// <summary>
/// Base class for Radio and RadioCard, providing selection state and group coordination.
/// </summary>
public abstract class RadioSelectableComponentBase : ShadcnComponentBase
{
    /// <summary>
    /// The parent RadioGroup when used inside a group.
    /// </summary>
    [CascadingParameter]
    internal RadioGroup? ParentGroup { get; set; }

    /// <summary>
    /// Whether this option is checked (when not in a group).
    /// </summary>
    [Parameter]
    public bool Checked { get; set; }

    /// <summary>
    /// Callback invoked when checked state changes (when not in a group).
    /// </summary>
    [Parameter]
    public EventCallback<bool> CheckedChanged { get; set; }

    /// <summary>
    /// The value of this option when selected.
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// The size of the radio indicator.
    /// </summary>
    [Parameter]
    public Size Size { get; set; } = Size.Md;

    /// <summary>
    /// Whether this option is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Whether this option is inside a RadioGroup.
    /// </summary>
    protected bool IsInGroup => ParentGroup is not null;

    /// <summary>
    /// Whether this option is currently selected.
    /// </summary>
    protected bool IsChecked => IsInGroup
        ? Value is not null && string.Equals(ParentGroup!.Value, Value, StringComparison.Ordinal)
        : Checked;

    /// <summary>
    /// Whether this option is disabled (either directly or via parent group).
    /// </summary>
    protected bool IsDisabled => Disabled || (ParentGroup?.Disabled ?? false);

    /// <summary>
    /// The effective size (from group or own Size).
    /// </summary>
    protected Size EffectiveSize => ParentGroup?.Size ?? Size;

    /// <summary>
    /// Selects this option (updates group value or Checked).
    /// </summary>
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
