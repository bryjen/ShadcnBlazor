using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Shared;
using ShadcnBlazor.Shared.Attributes;
using ShadcnBlazor.Shared.Enums;

namespace ShadcnBlazor.Components.Avatar;

[ComponentMetadata(Name = nameof(Avatar), Description = "Displays image avatars with text fallback for missing or loading images.", Dependencies = [])]
public partial class Avatar : ShadcnComponentBase
{
    [Parameter] public string? Src { get; set; }
    [Parameter] public string? Alt { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public Size Size { get; set; } = Size.Md;

    private string? _lastSrc;
    private bool _imageFailed;

    private bool ShouldRenderImage => !string.IsNullOrWhiteSpace(Src) && !_imageFailed;

    private string GetSizeClass() => Size switch
    {
        Size.Xs => "size-5 text-[10px]",
        Size.Sm => "size-6 text-xs",
        Size.Md => "size-8 text-sm",
        Size.Lg => "size-10 text-base",
        _ => "size-8 text-sm",
    };

    private string GetClass()
    {
        var baseClasses = "bg-background text-primary font-bold inline-flex shrink-0 items-center justify-center overflow-hidden rounded-full leading-none uppercase select-none";
        return MergeCss(baseClasses, GetSizeClass(), Class);
    }

    private string GetImageClass()
    {
        var baseClasses = "block size-full object-cover";
        return MergeCss(baseClasses);
    }

    private string GetFallbackText()
    {
        if (string.IsNullOrWhiteSpace(Alt))
        {
            return "?";
        }

        var words = Alt
            .Split([' ', '-', '_'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(word => word.Length > 0)
            .Take(2)
            .ToArray();

        if (words.Length == 0)
        {
            return "?";
        }

        if (words.Length == 1)
        {
            var token = words[0];
            return token.Length >= 2 ? token[..2].ToUpperInvariant() : token[..1].ToUpperInvariant();
        }

        var first = words[0][..1];
        var second = words[1][..1];
        return string.Concat(first, second).ToUpperInvariant();
    }

    protected override void OnParametersSet()
    {
        if (!string.Equals(_lastSrc, Src, StringComparison.Ordinal))
        {
            _imageFailed = false;
            _lastSrc = Src;
        }
    }

    private void HandleImageError()
    {
        _imageFailed = true;
    }
}
