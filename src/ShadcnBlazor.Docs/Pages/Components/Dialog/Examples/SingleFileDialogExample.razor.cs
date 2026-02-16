using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Dialog;
using ShadcnBlazor.Components.Dialog.Models;

namespace ShadcnBlazor.Docs.Pages.Components.Dialog.Examples;

/// <summary>
/// Dialog content component defined in the same file as the trigger (code-behind).
/// Keeps trigger and dialog in one logical unit.
/// </summary>
public class SingleFileDialogContent : ComponentBase
{
    [CascadingParameter]
    public IDialogInstance? Dialog { get; set; }

    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "grid gap-4 py-4");
        builder.OpenElement(2, "p");
        builder.AddAttribute(3, "class", "text-sm text-muted-foreground");
        builder.AddContent(4, "This dialog is defined in the same file as the trigger.");
        builder.CloseElement();
        builder.OpenElement(5, "div");
        builder.AddAttribute(6, "class", "flex justify-end gap-2");
        builder.OpenElement(7, "button");
        builder.AddAttribute(8, "class", "inline-flex items-center justify-center rounded-md text-sm font-medium ring-offset-background transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50 border border-input bg-background hover:bg-accent hover:text-accent-foreground h-10 px-4 py-2");
        builder.AddAttribute(9, "onclick", EventCallback.Factory.Create(this, () => Dialog?.Cancel()));
        builder.AddContent(10, "Cancel");
        builder.CloseElement();
        builder.OpenElement(11, "button");
        builder.AddAttribute(12, "class", "inline-flex items-center justify-center rounded-md text-sm font-medium ring-offset-background transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50 bg-primary text-primary-foreground hover:bg-primary/90 h-10 px-4 py-2");
        builder.AddAttribute(13, "onclick", EventCallback.Factory.Create(this, () => Dialog?.Close(DialogResult.Ok(true))));
        builder.AddContent(14, "Confirm");
        builder.CloseElement();
        builder.CloseElement();
        builder.CloseElement();
    }
}
