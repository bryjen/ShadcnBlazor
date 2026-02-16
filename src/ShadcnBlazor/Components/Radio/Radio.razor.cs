using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Shared.Attributes;
using ShadcnBlazor.Shared.Enums;

#pragma warning disable CS8524

namespace ShadcnBlazor.Components.Radio;

/// <summary>
/// Radio option for single selection within a RadioGroup.
/// </summary>
[ComponentMetadata(Name = nameof(Radio), Description = "Radio and RadioCard options for single selection within a RadioGroup.", Dependencies = [])]
public partial class Radio : RadioSelectableComponentBase
{
    /// <summary>
    /// Optional label content displayed next to the radio.
    /// </summary>
    [Parameter]
    public RenderFragment? LabelContent { get; set; }

    /// <summary>
    /// Content displayed next to the radio (alternative to LabelContent).
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// CSS classes for the radio button element.
    /// </summary>
    [Parameter]
    public string ButtonClass { get; set; } = string.Empty;

    /// <summary>
    /// Vertical alignment of the radio relative to its label.
    /// </summary>
    [Parameter]
    public VerticalAlignment Alignment { get; set; } = VerticalAlignment.Center;

    /// <summary>
    /// Whether the radio is in an invalid state. When true, sets <c>aria-invalid="true"</c> and applies invalid styling.
    /// </summary>
    [Parameter]
    public bool Invalid { get; set; }

    private string GetContainerClass()
    {
        var alignmentClass = Alignment switch
        {
            VerticalAlignment.Top => "items-start",
            VerticalAlignment.Center => "items-center",
            VerticalAlignment.Bottom => "items-end",
            _ => "items-center",
        };
        var invalidClass = Invalid ? "data-[invalid]:text-destructive" : "";
        var disabledClass = IsDisabled ? "data-[disabled]:text-muted-foreground data-[disabled]:cursor-not-allowed data-[disabled]:opacity-70" : "";
        return MergeCss("flex gap-2 cursor-pointer", alignmentClass, invalidClass, disabledClass, Class);
    }

    private string GetClass()
    {
        var baseClasses = "peer flex items-center justify-center p-0 border-input bg-input/30 data-[state=checked]:bg-primary data-[state=checked]:text-primary-foreground data-[state=checked]:border-primary focus-visible:border-ring focus-visible:ring-ring/50 aria-invalid:ring-destructive/40 aria-invalid:border-destructive shrink-0 rounded-full border shadow-xs transition-all duration-200 outline-none focus-visible:ring-[3px] disabled:cursor-not-allowed disabled:opacity-50";
        var sizeClasses = EffectiveSize switch
        {
            Size.Sm => "size-3",
            Size.Md => "size-4",
            Size.Lg => "size-5",
        };
        
        var alignmentClasses = Alignment switch
        {
            VerticalAlignment.Top => "mt-1",
            _ => string.Empty
        };

        return MergeCss(baseClasses, sizeClasses, alignmentClasses, ButtonClass);
    }

    private string GetIndicatorClass()
    {
        if (IsChecked)
        {
            return "grid place-content-center size-full text-current animate-in zoom-in-50 fade-in-0 duration-200";
        }

        return "grid place-content-center size-full text-current animate-out zoom-out-50 fade-out-0 duration-150 pointer-events-none opacity-0";
    }

    private string GetDotClass()
    {
        var sizeClasses = EffectiveSize switch
        {
            Size.Sm => "size-1.5",
            Size.Md => "size-2",
            Size.Lg => "size-2.5",
        };

        return MergeCss("rounded-full bg-current", sizeClasses);
    }

    private string GetAriaChecked()
    {
        return IsChecked ? "true" : "false";
    }

    private async Task HandleClick()
    {
        await SelectAsync();
    }
}




