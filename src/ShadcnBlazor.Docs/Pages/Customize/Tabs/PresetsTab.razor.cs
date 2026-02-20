using Microsoft.AspNetCore.Components;

namespace ShadcnBlazor.Docs.Pages.Customize.Tabs;

public partial class PresetsTab : ComponentBase
{
    private sealed record PresetRow(string Label, string Value, RenderFragment Icon);

    private List<PresetRow> TopItems =>
    [
        new("Preset", "Custom", IconDot()),
        new("Component Library", "Radix UI", IconGlyph("[]")),
        new("Style", "Nova", IconSquare()),
        new("Base Color", "Neutral", IconCircle()),
        new("Theme", "Neutral", IconCircle()),
        new("Icon Library", "Lucide", IconGlyph("@")),
        new("Font", "Inter", IconGlyph("Aa")),
        new("Radius", "Default", IconCorner()),
        new("Menu Color", "Default", IconGlyph("M")),
        new("Menu Accent", "Subtle", IconGlyph("*")),
    ];

    private List<PresetRow> BottomItems =>
    [
        new("Shuffle", "Try Random", IconGlyph("R")),
        new("Reset", "Start Over", IconGlyph("C")),
    ];

    private static RenderFragment IconDot() => b =>
    {
        b.OpenElement(0, "span");
        b.AddAttribute(1, "class", "inline-block size-2 rounded-full bg-muted-foreground/70");
        b.CloseElement();
    };

    private static RenderFragment IconCircle() => b =>
    {
        b.OpenElement(0, "span");
        b.AddAttribute(1, "class", "inline-block size-3.5 rounded-full bg-muted-foreground/70");
        b.CloseElement();
    };

    private static RenderFragment IconSquare() => b =>
    {
        b.OpenElement(0, "span");
        b.AddAttribute(1, "class", "inline-block size-3 rounded-sm border border-muted-foreground/70");
        b.CloseElement();
    };

    private static RenderFragment IconGlyph(string glyph) => b =>
    {
        b.OpenElement(0, "span");
        b.AddAttribute(1, "class", "inline-flex h-4 w-4 items-center justify-center rounded border border-border text-[10px] font-semibold");
        b.AddContent(2, glyph);
        b.CloseElement();
    };

    private static RenderFragment IconCorner() => b =>
    {
        b.OpenElement(0, "span");
        b.AddAttribute(1, "class", "inline-block w-3.5 h-3.5 border-l border-t border-muted-foreground/70 rounded-tl-sm");
        b.CloseElement();
    };
}
