using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Shared;
using ShadcnBlazor.Shared.Attributes;
using TailwindMerge;

namespace ShadcnBlazor.Components.Textarea;

[ComponentMetadata(Name = nameof(Textarea), Description = "", Dependencies = [])]
public partial class Textarea : ShadcnComponentBase
{
    [Parameter] 
    public string? Value { get; set; }
    
    [Parameter] 
    public EventCallback<string?> ValueChanged { get; set; }
    
    [Parameter] 
    public int Rows { get; set; } = 4;
    
    [Parameter] 
    public string? Placeholder { get; set; }
    
    [Parameter] 
    public bool Disabled { get; set; }
    
    public string GetClass()
    {
        var baseClasses = "border-input placeholder:text-muted-foreground focus-visible:border-ring focus-visible:ring-ring/50 aria-invalid:ring-destructive/20 dark:aria-invalid:ring-destructive/40 aria-invalid:border-destructive dark:bg-input/30 flex min-h-16 w-full rounded-md border bg-transparent px-3 py-2 text-base shadow-xs transition-[color,box-shadow] outline-none focus-visible:ring-[3px] disabled:cursor-not-allowed disabled:opacity-50 md:text-sm";
        return MergeCss(baseClasses, Class ?? "");
    }

    private async Task OnChange(ChangeEventArgs args)
    {
        var newValue = args.Value?.ToString();
        Value = newValue;
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(newValue);
        }
    }
}

