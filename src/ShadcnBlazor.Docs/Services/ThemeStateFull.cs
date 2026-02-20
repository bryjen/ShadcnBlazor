namespace ShadcnBlazor.Docs.Services;

/// <summary>
/// Full theme state with separate light, dark, and shared token sets.
/// </summary>
public sealed class ThemeStateFull
{
    /// <summary>
    /// Tokens applied in <c>:root</c> for light mode.
    /// </summary>
    public ThemeState Light { get; set; } = new();

    /// <summary>
    /// Tokens applied in <c>.dark</c> for dark mode.
    /// </summary>
    public ThemeState Dark { get; set; } = new();

    /// <summary>
    /// Shared tokens that are not mode-specific.
    /// </summary>
    public ThemeState Shared { get; set; } = new();

    /// <summary>
    /// Creates a deep copy of the current full theme state.
    /// </summary>
    public ThemeStateFull Clone() => new()
    {
        Light = Light.Clone(),
        Dark = Dark.Clone(),
        Shared = Shared.Clone()
    };

    /// <summary>
    /// Creates a full state from a dark-mode token set, using default light/shared values.
    /// </summary>
    public static ThemeStateFull FromDark(ThemeState dark)
    {
        var full = new ThemeStateFull
        {
            Dark = dark.Clone()
        };

        // Keep non-color tokens in sync with preset-provided values.
        full.Shared.Radius = dark.Radius;
        full.Shared.BackgroundBlack = dark.BackgroundBlack;
        full.Shared.BackgroundDark = dark.BackgroundDark;
        full.Shared.CardDark = dark.CardDark;
        full.Shared.BorderDark = dark.BorderDark;
        full.Shared.ScrollbarTrack = dark.ScrollbarTrack;
        full.Shared.ScrollbarThumb = dark.ScrollbarThumb;
        full.Shared.ScrollbarThumbHover = dark.ScrollbarThumbHover;
        full.Shared.ChatDark = dark.ChatDark;
        full.Shared.ChatSidebar = dark.ChatSidebar;
        full.Shared.ChatBorder = dark.ChatBorder;
        full.Shared.SidebarText = dark.SidebarText;
        full.Shared.SidebarHeader = dark.SidebarHeader;
        full.Shared.FontSans = dark.FontSans;
        full.Shared.FontSerif = dark.FontSerif;
        full.Shared.FontMono = dark.FontMono;
        full.Shared.TrackingNormal = dark.TrackingNormal;
        full.Shared.Spacing = dark.Spacing;
        full.Shared.ShadowX = dark.ShadowX;
        full.Shared.ShadowY = dark.ShadowY;
        full.Shared.ShadowBlur = dark.ShadowBlur;
        full.Shared.ShadowSpread = dark.ShadowSpread;
        full.Shared.ShadowOpacity = dark.ShadowOpacity;
        full.Shared.ShadowColor = dark.ShadowColor;
        full.Shared.Shadow2xs = dark.Shadow2xs;
        full.Shared.ShadowXs = dark.ShadowXs;
        full.Shared.ShadowSm = dark.ShadowSm;
        full.Shared.Shadow = dark.Shadow;
        full.Shared.ShadowMd = dark.ShadowMd;
        full.Shared.ShadowLg = dark.ShadowLg;
        full.Shared.ShadowXl = dark.ShadowXl;
        full.Shared.Shadow2xl = dark.Shadow2xl;

        return full;
    }
}
