using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;
using ShadcnBlazor.Components.Shared.Models.Enums;

namespace ShadcnBlazor.Components.Avatar;

/// <summary>
/// Displays a user avatar with optional image, fallback initials, and status badge.
/// </summary>
public partial class Avatar : ShadcnComponentBase
{
    /// <summary>The URL of the avatar image.</summary>
    [Parameter] public string? Src { get; set; }
    /// <summary>Alt text for the image; used to generate fallback initials when the image fails to load.</summary>
    [Parameter] public string? Alt { get; set; }
    /// <summary>Content rendered when no image is shown (e.g. initials or custom fallback).</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }
    /// <summary>Optional badge overlay, typically an <see cref="AvatarBadge"/>.</summary>
    [Parameter] public RenderFragment? Badge { get; set; }
    /// <summary>The size of the avatar.</summary>
    [Parameter] public Size Size { get; set; } = Size.Md;

    private string? _lastSrc;
    private bool _imageFailed;

    private bool ShouldRenderImage => !string.IsNullOrWhiteSpace(Src) && !_imageFailed;

    private string GetSizeClass() => Size switch
    {
        Size.Sm => "size-6 text-[10px]",
        Size.Md => "size-8 text-xs",
        Size.Lg => "size-10 text-sm",
        _ => "size-8 text-xs",
    };

    private string GetClass()
    {
        var baseClasses = "relative bg-background text-primary font-bold inline-flex shrink-0 items-center justify-center overflow-hidden rounded-full leading-none uppercase select-none";
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

    /// <inheritdoc />
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
