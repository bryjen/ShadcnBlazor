using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ShadcnBlazor.Shared;
using ShadcnBlazor.Shared.Enums;
using TailwindMerge;

namespace ShadcnBlazor.Components.Button;

public partial class ToggleButton : ShadcnComponentBase
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public Variant VariantUntoggled { get; set; } = Variant.Outline;

    [Parameter]
    public Variant VariantToggled { get; set; } = Variant.Default;

    [Parameter]
    public Size Size { get; set; } = Size.Md;

    [Parameter]
    public ButtonType Type { get; set; } = ButtonType.Button;

    [Parameter]
    public bool Disabled { get; set; }

    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    [Parameter]
    public bool IsToggled { get; set; }

    [Parameter]
    public EventCallback<bool> IsToggledChanged { get; set; }

    private bool _localToggled;

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