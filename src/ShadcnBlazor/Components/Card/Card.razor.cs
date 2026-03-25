using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Components.Card;

/// <summary>
/// A container card component for grouping related content.
/// </summary>
public partial class Card : ShadcnComponentBase
{
    /// <summary>The content of the card.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string GetClass()
    {
        return MergeCss("flex flex-col gap-6 rounded-xl border border-border bg-transparent text-card-foreground shadow-sm transition-shadow duration-200 hover:shadow-md", Class);
    }
}

/// <summary>
/// Header section of a card.
/// </summary>
public partial class CardHeader : ShadcnComponentBase
{
    /// <summary>The content of the card header.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string GetClass()
    {
        return MergeCss("grid auto-rows-min grid-rows-[auto_auto] items-start gap-2 -mx-6 px-6 -mt-6 pt-6 pb-6 border-b border-border space-y-1.5", Class);
    }
}

/// <summary>
/// Title element within a card header.
/// </summary>
public partial class CardTitle : ShadcnComponentBase
{
    /// <summary>The content of the card title.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string GetClass()
    {
        return MergeCss("leading-none font-semibold", Class);
    }
}

/// <summary>
/// Description element within a card header.
/// </summary>
public partial class CardDescription : ShadcnComponentBase
{
    /// <summary>The content of the card description.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string GetClass()
    {
        return MergeCss("text-sm text-muted-foreground", Class);
    }
}

/// <summary>
/// Main content section of a card.
/// </summary>
public partial class CardContent : ShadcnComponentBase
{
    /// <summary>The content of the card.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string GetClass()
    {
        return MergeCss("flex flex-col flex-1 gap-6", Class);
    }
}

/// <summary>
/// Footer section of a card.
/// </summary>
public partial class CardFooter : ShadcnComponentBase
{
    /// <summary>The content of the card footer.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string GetClass()
    {
        return MergeCss("flex items-center -mx-6 px-6 -mb-6 pb-6 pt-6 border-t border-border bg-muted/50", Class);
    }
}

/// <summary>
/// Action area within a card header, positioned in top-right.
/// </summary>
public partial class CardAction : ShadcnComponentBase
{
    /// <summary>The content of the card action area.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string GetClass()
    {
        return MergeCss("col-start-2 row-span-2 row-start-1 self-start justify-self-end", Class);
    }
}
