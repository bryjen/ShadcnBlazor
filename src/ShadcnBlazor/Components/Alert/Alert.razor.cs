using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Shared;
using ShadcnBlazor.Shared.Attributes;

namespace ShadcnBlazor.Components.Alert;

[ComponentMetadata(Name = nameof(Alert), Description = "", Dependencies = [])]
public partial class Alert
{
    [Parameter] 
    public RenderFragment? ChildContent { get; set; }
    
    [Parameter] 
    public string Variant { get; set; } = "default";
    
    [Parameter] 
    public string? Class { get; set; }
    
    [Parameter(CaptureUnmatchedValues = true)] 
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClass()
    {
        var baseClasses = "relative w-full rounded-lg border px-4 py-3 text-sm grid has-[>svg]:grid-cols-[calc(var(--spacing)*4)_1fr] grid-cols-[0_1fr] has-[>svg]:gap-x-3 gap-y-0.5 items-start [&>svg]:size-4 [&>svg]:translate-y-0.5 animate-in fade-in-0 slide-in-from-top-1 duration-200";
        
        var variantClasses = Variant switch
        {
            "default" => "bg-card text-card-foreground border-border [&>svg]:text-card-foreground",
            "destructive" => "bg-card text-card-foreground border-destructive [&>svg]:text-destructive *:data-[slot=alert-description]:text-destructive/90",
            _ => "bg-card text-card-foreground border-border [&>svg]:text-card-foreground"
        };
        
        return ClassBuilder.Merge(baseClasses, variantClasses, Class);
    }
}

