using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Shared;
using ShadcnBlazor.Shared.Attributes;

namespace ShadcnBlazor.Components.Avatar;

[ComponentMetadata(Name = nameof(Avatar), Description = "", Dependencies = [])]
public partial class Avatar
{
    [Parameter] 
    public RenderFragment? ChildContent { get; set; }
    
    [Parameter] 
    public string Size { get; set; } = "default";
    
    [Parameter] 
    public string? Class { get; set; }
    
    [Parameter(CaptureUnmatchedValues = true)] 
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClass()
    {
        var baseClasses = "group/avatar relative flex size-8 shrink-0 overflow-hidden rounded-full select-none data-[size=lg]:size-10 data-[size=sm]:size-6";
        return ClassBuilder.Merge(baseClasses, Class);
    }
    
    public bool ShowFallback { get; set; } = false;
    private string? _imageSrc;

    public void OnImageError()
    {
        ShowFallback = true;
        StateHasChanged();
    }

    public string? GetImageSrc() => _imageSrc;

    public void SetImageSrc(string? src)
    {
        _imageSrc = src;
        ShowFallback = false;
    }
}
