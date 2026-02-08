using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ShadcnBlazor.ComponentDependencies;

namespace ShadcnBlazor.Components.Button;

public partial class ToggleButton : ComponentBase
{
    [Parameter] 
    public RenderFragment? ChildContent { get; set; }
    
    [Parameter] 
    public string VariantUntoggled { get; set; } = "outline";
    
    [Parameter] 
    public string VariantToggled { get; set; } = "default";
    
    [Parameter] 
    public string Size { get; set; } = "default";
    
    [Parameter] 
    public string Type { get; set; } = "button";
    
    [Parameter] 
    public bool Disabled { get; set; }
    
    [Parameter] 
    public string? Class { get; set; }
    
    [Parameter] 
    public EventCallback<MouseEventArgs> OnClick { get; set; }
    
    [Parameter] 
    public bool IsToggled { get; set; }
    
    [Parameter] 
    public EventCallback<bool> IsToggledChanged { get; set; }
    
    [Parameter(CaptureUnmatchedValues = true)] 
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private bool _localToggled;
    private bool _initialized;

    protected override void OnParametersSet()
    {
        if (!_initialized)
        {
            _localToggled = IsToggled;
            _initialized = true;
            return;
        }

        if (IsControlled)
        {
            _localToggled = IsToggled;
        }
    }

    private bool IsControlled => IsToggledChanged.HasDelegate;

    private bool CurrentState => IsControlled ? IsToggled : _localToggled;

    private string CurrentVariant => CurrentState ? VariantToggled : VariantUntoggled;

    private string CurrentClass
    {
        get
        {
            if (VariantUntoggled == "outline")
            {
                var borderClass = CurrentState ? "border border-primary" : "border border-input/60";
                return ClassBuilder.Merge(Class, borderClass);
            }

            return ClassBuilder.Merge(Class);
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