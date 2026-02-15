using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Shared;
using ShadcnBlazor.Shared.Attributes;
using ShadcnBlazor.Shared.Enums;

namespace ShadcnBlazor.Components.Switch;

/// <summary>
/// Toggle switch for boolean on/off values.
/// </summary>
[ComponentMetadata(Name = nameof(Switch), Description = "Toggle switch for boolean on/off values.", Dependencies = [])]
public partial class Switch : ShadcnComponentBase
{
    /// <summary>
    /// Content to display alongside the switch (e.g., label).
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Whether the switch is on.
    /// </summary>
    [Parameter]
    public bool Checked { get; set; }

    /// <summary>
    /// Callback invoked when the checked state changes.
    /// </summary>
    [Parameter]
    public EventCallback<bool> CheckedChanged { get; set; }

    /// <summary>
    /// The size of the switch.
    /// </summary>
    [Parameter]
    public Size Size { get; set; } = Size.Md;

    /// <summary>
    /// Whether the switch is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    private string GetTrackClass()
    {
        var baseClasses = "peer inline-flex shrink-0 cursor-pointer items-center rounded-full border-2 border-transparent shadow-sm transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 focus-visible:ring-offset-background disabled:cursor-not-allowed disabled:opacity-50 data-[state=checked]:bg-primary data-[state=unchecked]:bg-input";
        var sizeClasses = Size switch
        {
            Size.Sm => "h-4 w-7",
            Size.Md => "h-5 w-9",
            Size.Lg => "h-6 w-11",
            _ => "h-5 w-9",
        };
        return MergeCss(baseClasses, sizeClasses, Class ?? "");
    }

    private string GetThumbClass()
    {
        var baseClasses = "pointer-events-none block rounded-full bg-background shadow-lg ring-0 transition-transform";
        var sizeClasses = Size switch
        {
            Size.Sm => "h-3 w-3 group-data-[state=checked]:translate-x-3 group-data-[state=unchecked]:translate-x-0",
            Size.Md => "h-4 w-4 group-data-[state=checked]:translate-x-4 group-data-[state=unchecked]:translate-x-0",
            Size.Lg => "h-5 w-5 group-data-[state=checked]:translate-x-5 group-data-[state=unchecked]:translate-x-0",
            _ => "h-4 w-4 group-data-[state=checked]:translate-x-4 group-data-[state=unchecked]:translate-x-0",
        };
        return MergeCss(baseClasses, sizeClasses);
    }

    private async Task HandleClick()
    {
        if (!Disabled)
        {
            Checked = !Checked;
            await CheckedChanged.InvokeAsync(Checked);
        }
    }
}
