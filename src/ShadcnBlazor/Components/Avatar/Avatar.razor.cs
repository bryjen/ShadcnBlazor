using ShadcnBlazor.Shared.Attributes;

namespace ShadcnBlazor.Components.Avatar;

[ComponentMetadata(Name = nameof(Avatar), Description = "", Dependencies = [])]
public partial class Avatar
{
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
