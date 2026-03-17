using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Components.Avatar;

/// <summary>
/// A badge indicator for the Avatar component, typically positioned at the bottom right.
/// Use the <c>Class</c> parameter to customize appearance (e.g. <c>bg-green-600 dark:bg-green-800</c>).
/// </summary>
public partial class AvatarBadge : ShadcnComponentBase
{
    /// <summary>The content of the badge (e.g. a colored dot or status indicator).</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private string GetClass()
    {
        var baseClasses = "absolute bottom-0 right-0 size-2 rounded-full border-2 border-background bg-green-500 dark:bg-green-600";
        return MergeCss(baseClasses, Class);
    }
}
