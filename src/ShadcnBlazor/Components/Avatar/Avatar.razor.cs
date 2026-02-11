using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Shared;
using ShadcnBlazor.Shared.Attributes;
using ShadcnBlazor.Shared.Enums;

namespace ShadcnBlazor.Components.Avatar;

[ComponentMetadata(Name = nameof(Avatar), Description = "Display text avatars", Dependencies = [])]
public partial class Avatar : ShadcnComponentBase
{
    [Parameter] public string? Text { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public Size Size { get; set; } = Size.Md;

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
        var baseClasses = "bg-muted text-muted-foreground inline-flex shrink-0 items-center justify-center rounded-full font-semibold leading-none uppercase select-none";
        return MergeCss(baseClasses, GetSizeClass(), Class);
    }
}
