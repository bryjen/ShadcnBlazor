using Microsoft.AspNetCore.Components;
using TailwindMerge;

namespace ShadcnBlazor.Shared;

public abstract class ShadcnComponentBase : ComponentBase
{
    [Inject] 
    public required TwMerge TwMerge { get; set; }
    
    [Parameter] 
    public string Class { get; set; } = string.Empty;

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }
    
    protected string MergeCss(params string[] classes)
    {
        var joined = string.Join(" ", classes) + " " + Class;
        return TwMerge.Merge(joined) ?? joined;
    }
}
