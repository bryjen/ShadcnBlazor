using ShadcnBlazor.Components.Shared.Models.Enums;

#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.

namespace ShadcnBlazor.Components.Button;

internal static class ButtonStyles
{
    public static string Build(Func<string[], string> mergeCallback, Variant variant, Size size, ButtonState? state, string @class)
    {
        var baseClasses = string.Join(" ",
            "inline-flex items-center justify-center gap-2 shrink-0 whitespace-nowrap",
            "text-sm font-medium rounded-md",
            "transition-all duration-200 active:scale-95 cursor-pointer",
            "disabled:pointer-events-none disabled:opacity-50",
            "outline-none focus-visible:border-ring focus-visible:ring-ring/50 focus-visible:ring-[3px]",
            "aria-invalid:ring-destructive/40 aria-invalid:border-destructive",
            "[&_svg]:pointer-events-none [&_svg:not([class*='size-'])]:size-4 [&_svg]:shrink-0"
        );
        
        var stateClasses = state switch
        {
            ButtonState.Loading => "cursor-wait",
            ButtonState.Disabled => "cursor-not-allowed",
            null => string.Empty,
        };

        var variantClasses = variant switch
        {
            Variant.Default => "bg-primary text-primary-foreground hover:bg-primary/90",
            Variant.Destructive => "bg-destructive/60 text-white hover:bg-destructive/90 focus-visible:ring-destructive/40",
            Variant.Outline => "border bg-background shadow-xs hover:bg-accent hover:text-accent-foreground bg-input/30 border-input hover:bg-input/50",
            Variant.Secondary => "bg-secondary text-secondary-foreground hover:bg-secondary/80",
            Variant.Ghost => "hover:bg-accent hover:text-accent-foreground hover:bg-accent/50",
            Variant.Link => "text-primary underline-offset-4 hover:underline",
        };

        var sizeClasses = size switch
        {
            Size.Sm => "h-5 gap-1 rounded-md px-1.75 py-0.75 text-[0.6rem] has-[>svg]:px-1.5 [&_svg:not([class*='size-'])]:size-2.5",
            Size.Md => "h-7 rounded-md gap-1.5 px-2.75 py-1.25 has-[>svg]:px-2.25 [&_svg:not([class*='size-'])]:size-3",
            Size.Lg => "h-8 rounded-md px-3.5 py-1.75 has-[>svg]:px-2.75 [&_svg:not([class*='size-'])]:size-3.5",
        };

        return mergeCallback([baseClasses, variantClasses, sizeClasses, stateClasses, @class]);
    }
}

/// <summary>
/// The state of the button (loading, disabled, or normal when null).
/// </summary>
public enum ButtonState
{
    /// <summary>Button is loading; disabled with aria-busy.</summary>
    Loading,
    /// <summary>Button is disabled.</summary>
    Disabled,
}

/// <summary>
/// The HTML button type attribute value.
/// </summary>
public enum ButtonType
{
    /// <summary>Standard clickable button with no default behavior.</summary>
    Button,
    /// <summary>Submits the containing form.</summary>
    Submit,
    /// <summary>Resets the containing form to its initial state.</summary>
    Reset
}