using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ShadcnBlazor.Components.Shared;
using ShadcnBlazor.Components.Shared.Models.Enums;

namespace ShadcnBlazor.Components.Button;

/// <summary>
/// A button that toggles between two visual states (on/off).
/// </summary>
public partial class ToggleButton : ShadcnComponentBase
{
    /// <summary>The content rendered inside the button.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>The variant when the button is not toggled.</summary>
    [Parameter]
    public Variant VariantUntoggled { get; set; } = Variant.Outline;

    /// <summary>The variant when the button is toggled on.</summary>
    [Parameter]
    public Variant VariantToggled { get; set; } = Variant.Default;

    /// <summary>The size of the button.</summary>
    [Parameter]
    public Size Size { get; set; } = Size.Md;

    /// <summary>The HTML button type.</summary>
    [Parameter]
    public ButtonType Type { get; set; } = ButtonType.Button;

    /// <summary>When true, the button is disabled.</summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>Callback fired when the button is clicked.</summary>
    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>Whether the button is currently toggled on. Use with <see cref="IsToggledChanged"/> for two-way binding.</summary>
    [Parameter]
    public bool IsToggled { get; set; }

    /// <summary>Callback fired when the toggled state changes. Use with <see cref="IsToggled"/> for two-way binding.</summary>
    [Parameter]
    public EventCallback<bool> IsToggledChanged { get; set; }

    private bool _localToggled;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        _localToggled = IsToggled;
    }

    private bool IsControlled => IsToggledChanged.HasDelegate;

    private bool CurrentState => IsControlled ? IsToggled : _localToggled;

    private Variant CurrentVariant => CurrentState ? VariantToggled : VariantUntoggled;

    private string CurrentClass
    {
        get
        {
            if (VariantUntoggled == Variant.Outline)
            {
                var borderClass = CurrentState ? "border border-primary" : "border border-input/60";
                return MergeCss(Class ?? "", borderClass);
            }

            return Class ?? "";
        }
    }

    private async Task HandleClick(MouseEventArgs args)
    {
        var newState = !CurrentState;
        if (!IsControlled)
        {
            _localToggled = newState;
        }

        if (IsToggledChanged.HasDelegate)
        {
            await IsToggledChanged.InvokeAsync(newState);
        }

        if (OnClick.HasDelegate)
        {
            await OnClick.InvokeAsync(args);
        }

        StateHasChanged();
    }
}