using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Docs.Components.Samples.MarkdownEditor.Models;

namespace ShadcnBlazor.Docs.Components.Samples.MarkdownEditor.Components;

public partial class TimelineItem : ComponentBase
{
    [Parameter]
    public required TimelineItemModel TimelineItemModel { get; set; }

    [Parameter]
    public required RenderFragment AvatarRenderFragment { get; set; }

    [Parameter]
    public bool IsHeaderItem { get; set; }

    [Parameter]
    public bool IsAuthorIssueAuthor { get; set; }

    [Parameter]
    public bool IsAuthorRepoOwner { get; set; }

    private static string PublishTimespanToString(TimeSpan publishTimespan)
    {
        return publishTimespan switch
        {
            _ when publishTimespan.Days > 1 => $"{publishTimespan.Days} days",
            _ when publishTimespan.Days == 1 => $"{publishTimespan.Days} day",
            _ when publishTimespan.Hours > 1 => $"{publishTimespan.Hours} hours",
            _ when publishTimespan.Hours == 1 => $"{publishTimespan.Hours} hour",
            _ when publishTimespan.Minutes > 1 => $"{publishTimespan.Minutes} minutes",
            _ when publishTimespan.Minutes == 1 => $"{publishTimespan.Minutes} minute",
            _ when publishTimespan.Seconds > 1 => $"{publishTimespan.Seconds} seconds",
            _ when publishTimespan.Seconds == 1 => $"{publishTimespan.Seconds} second",
            _ => "Error"
        };
    }
}


