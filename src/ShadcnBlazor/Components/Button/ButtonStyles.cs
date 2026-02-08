using ShadcnBlazor.ComponentDependencies;

namespace ShadcnBlazor.Components.Button;

internal static class ButtonStyles
{
    public static string Build(string variant, string size, string? additional)
    {
        var baseClasses = "inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-md text-sm font-medium transition-all duration-200 disabled:pointer-events-none disabled:opacity-50 [&_svg]:pointer-events-none [&_svg:not([class*='size-'])]:size-4 shrink-0 [&_svg]:shrink-0 outline-none focus-visible:border-ring focus-visible:ring-ring/50 focus-visible:ring-[3px] aria-invalid:ring-destructive/40 aria-invalid:border-destructive active:scale-95 cursor-pointer";

        var variantClasses = variant switch
        {
            "default" => "bg-primary text-primary-foreground hover:bg-primary/90",
            "destructive" => "bg-destructive/60 text-white hover:bg-destructive/90 focus-visible:ring-destructive/40",
            "outline" => "border bg-background shadow-xs hover:bg-accent hover:text-accent-foreground bg-input/30 border-input hover:bg-input/50",
            "secondary" => "bg-secondary text-secondary-foreground hover:bg-secondary/80",
            "ghost" => "hover:bg-accent hover:text-accent-foreground hover:bg-accent/50",
            "link" => "text-primary underline-offset-4 hover:underline",
            _ => "bg-primary text-primary-foreground hover:bg-primary/90"
        };

        var sizeClasses = size switch
        {
            "default" => "h-9 px-4 py-2 has-[>svg]:px-3",
            "xs" => "h-6 gap-1 rounded-md px-2 text-xs has-[>svg]:px-1.5 [&_svg:not([class*='size-'])]:size-3",
            "sm" => "h-8 rounded-md gap-1.5 px-3 has-[>svg]:px-2.5",
            "lg" => "h-10 rounded-md px-6 has-[>svg]:px-4",
            "icon" => "size-9",
            "icon-xs" => "size-6 rounded-md [&_svg:not([class*='size-'])]:size-3",
            "icon-sm" => "size-8",
            "icon-lg" => "size-10",
            _ => "h-9 px-4 py-2 has-[>svg]:px-3"
        };

        return ClassBuilder.Merge(baseClasses, variantClasses, sizeClasses, additional);
    }
}
