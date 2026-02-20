namespace ShadcnBlazor.Docs.Services;

/// <summary>
/// Represents the current theme values for the docs app.
/// </summary>
public sealed class ThemeState
{
    /// <summary>Base radius token.</summary>
    public string Radius { get; set; } = "0.65rem";

    /// <summary>Base background color.</summary>
    public string Background { get; set; } = "oklch(1 0 0)";
    /// <summary>Base foreground color.</summary>
    public string Foreground { get; set; } = "oklch(0.141 0.005 285.823)";
    /// <summary>Card background color.</summary>
    public string Card { get; set; } = "oklch(1 0 0)";
    /// <summary>Card foreground color.</summary>
    public string CardForeground { get; set; } = "oklch(0.141 0.005 285.823)";
    /// <summary>Popover background color.</summary>
    public string Popover { get; set; } = "oklch(1 0 0)";
    /// <summary>Popover foreground color.</summary>
    public string PopoverForeground { get; set; } = "oklch(0.141 0.005 285.823)";
    /// <summary>Primary color.</summary>
    public string Primary { get; set; } = "oklch(0.488 0.243 264.376)";
    /// <summary>Primary foreground color.</summary>
    public string PrimaryForeground { get; set; } = "oklch(0.97 0.014 254.604)";
    /// <summary>Secondary color.</summary>
    public string Secondary { get; set; } = "oklch(0.967 0.001 286.375)";
    /// <summary>Secondary foreground color.</summary>
    public string SecondaryForeground { get; set; } = "oklch(0.21 0.006 285.885)";
    /// <summary>Muted color.</summary>
    public string Muted { get; set; } = "oklch(0.967 0.001 286.375)";
    /// <summary>Muted foreground color.</summary>
    public string MutedForeground { get; set; } = "oklch(0.552 0.016 285.938)";
    /// <summary>Accent color.</summary>
    public string Accent { get; set; } = "oklch(0.967 0.001 286.375)";
    /// <summary>Accent foreground color.</summary>
    public string AccentForeground { get; set; } = "oklch(0.21 0.006 285.885)";
    /// <summary>Destructive color.</summary>
    public string Destructive { get; set; } = "oklch(0.577 0.245 27.325)";
    /// <summary>Destructive foreground color.</summary>
    public string DestructiveForeground { get; set; } = "oklch(1 0 0)";
    /// <summary>Border color.</summary>
    public string Border { get; set; } = "oklch(0.92 0.004 286.32)";
    /// <summary>Input color.</summary>
    public string Input { get; set; } = "oklch(0.92 0.004 286.32)";
    /// <summary>Ring color.</summary>
    public string Ring { get; set; } = "oklch(0.708 0 0)";
    /// <summary>Chart color 1.</summary>
    public string Chart1 { get; set; } = "oklch(0.809 0.105 251.813)";
    /// <summary>Chart color 2.</summary>
    public string Chart2 { get; set; } = "oklch(0.623 0.214 259.815)";
    /// <summary>Chart color 3.</summary>
    public string Chart3 { get; set; } = "oklch(0.546 0.245 262.881)";
    /// <summary>Chart color 4.</summary>
    public string Chart4 { get; set; } = "oklch(0.488 0.243 264.376)";
    /// <summary>Chart color 5.</summary>
    public string Chart5 { get; set; } = "oklch(0.424 0.199 265.638)";
    /// <summary>Sidebar background.</summary>
    public string Sidebar { get; set; } = "oklch(0.985 0 0)";
    /// <summary>Sidebar foreground.</summary>
    public string SidebarForeground { get; set; } = "oklch(0.141 0.005 285.823)";
    /// <summary>Sidebar primary color.</summary>
    public string SidebarPrimary { get; set; } = "oklch(0.546 0.245 262.881)";
    /// <summary>Sidebar primary foreground.</summary>
    public string SidebarPrimaryForeground { get; set; } = "oklch(0.97 0.014 254.604)";
    /// <summary>Sidebar accent color.</summary>
    public string SidebarAccent { get; set; } = "oklch(0.967 0.001 286.375)";
    /// <summary>Sidebar accent foreground.</summary>
    public string SidebarAccentForeground { get; set; } = "oklch(0.21 0.006 285.885)";
    /// <summary>Sidebar border color.</summary>
    public string SidebarBorder { get; set; } = "oklch(0.92 0.004 286.32)";
    /// <summary>Sidebar ring color.</summary>
    public string SidebarRing { get; set; } = "oklch(0.708 0 0)";

    /// <summary>Extended background black token.</summary>
    public string BackgroundBlack { get; set; } = "oklch(0 0 0)";
    /// <summary>Extended background dark token.</summary>
    public string BackgroundDark { get; set; } = "oklch(0.145 0 0)";
    /// <summary>Extended card dark token.</summary>
    public string CardDark { get; set; } = "oklch(0.19 0 0)";
    /// <summary>Extended border dark token.</summary>
    public string BorderDark { get; set; } = "oklch(0.255 0 0)";
    /// <summary>Scrollbar track color.</summary>
    public string ScrollbarTrack { get; set; } = "oklch(0.18 0 0)";
    /// <summary>Scrollbar thumb color.</summary>
    public string ScrollbarThumb { get; set; } = "oklch(0.35 0 0)";
    /// <summary>Scrollbar thumb hover color.</summary>
    public string ScrollbarThumbHover { get; set; } = "oklch(0.40 0 0)";

    /// <summary>Chat dark background token.</summary>
    public string ChatDark { get; set; } = "oklch(0.155 0 0)";
    /// <summary>Chat sidebar background token.</summary>
    public string ChatSidebar { get; set; } = "oklch(0.195 0 0)";
    /// <summary>Chat border token.</summary>
    public string ChatBorder { get; set; } = "oklch(0.255 0 0)";
    /// <summary>Sidebar text token.</summary>
    public string SidebarText { get; set; } = "oklch(0.82 0 0)";
    /// <summary>Sidebar header token.</summary>
    public string SidebarHeader { get; set; } = "oklch(0.575 0 0)";

    /// <summary>Sans font token.</summary>
    public string FontSans { get; set; } = "Inter, sans-serif";
    /// <summary>Serif font token.</summary>
    public string FontSerif { get; set; } = "Source Serif 4, serif";
    /// <summary>Monospace font token.</summary>
    public string FontMono { get; set; } = "JetBrains Mono, monospace";
    /// <summary>Normal tracking token.</summary>
    public string TrackingNormal { get; set; } = "0em";

    /// <summary>Base spacing token.</summary>
    public string Spacing { get; set; } = "0.25rem";

    /// <summary>Shadow X offset token.</summary>
    public string ShadowX { get; set; } = "0";
    /// <summary>Shadow Y offset token.</summary>
    public string ShadowY { get; set; } = "1px";
    /// <summary>Shadow blur token.</summary>
    public string ShadowBlur { get; set; } = "3px";
    /// <summary>Shadow spread token.</summary>
    public string ShadowSpread { get; set; } = "0px";
    /// <summary>Shadow opacity token.</summary>
    public string ShadowOpacity { get; set; } = "0.1";
    /// <summary>Shadow color token.</summary>
    public string ShadowColor { get; set; } = "oklch(0 0 0)";
    /// <summary>Shadow 2xs token.</summary>
    public string Shadow2xs { get; set; } = "0 1px 3px 0px hsl(0 0% 0% / 0.05)";
    /// <summary>Shadow xs token.</summary>
    public string ShadowXs { get; set; } = "0 1px 3px 0px hsl(0 0% 0% / 0.05)";
    /// <summary>Shadow sm token.</summary>
    public string ShadowSm { get; set; } = "0 1px 3px 0px hsl(0 0% 0% / 0.10), 0 1px 2px -1px hsl(0 0% 0% / 0.10)";
    /// <summary>Shadow token.</summary>
    public string Shadow { get; set; } = "0 1px 3px 0px hsl(0 0% 0% / 0.10), 0 1px 2px -1px hsl(0 0% 0% / 0.10)";
    /// <summary>Shadow md token.</summary>
    public string ShadowMd { get; set; } = "0 1px 3px 0px hsl(0 0% 0% / 0.10), 0 2px 4px -1px hsl(0 0% 0% / 0.10)";
    /// <summary>Shadow lg token.</summary>
    public string ShadowLg { get; set; } = "0 1px 3px 0px hsl(0 0% 0% / 0.10), 0 4px 6px -1px hsl(0 0% 0% / 0.10)";
    /// <summary>Shadow xl token.</summary>
    public string ShadowXl { get; set; } = "0 1px 3px 0px hsl(0 0% 0% / 0.10), 0 8px 10px -1px hsl(0 0% 0% / 0.10)";
    /// <summary>Shadow 2xl token.</summary>
    public string Shadow2xl { get; set; } = "0 1px 3px 0px hsl(0 0% 0% / 0.25)";

    /// <summary>
    /// Creates a deep copy of the current state.
    /// </summary>
    public ThemeState Clone() => new()
    {
        Radius = Radius,
        Background = Background,
        Foreground = Foreground,
        Card = Card,
        CardForeground = CardForeground,
        Popover = Popover,
        PopoverForeground = PopoverForeground,
        Primary = Primary,
        PrimaryForeground = PrimaryForeground,
        Secondary = Secondary,
        SecondaryForeground = SecondaryForeground,
        Muted = Muted,
        MutedForeground = MutedForeground,
        Accent = Accent,
        AccentForeground = AccentForeground,
        Destructive = Destructive,
        DestructiveForeground = DestructiveForeground,
        Border = Border,
        Input = Input,
        Ring = Ring,
        Chart1 = Chart1,
        Chart2 = Chart2,
        Chart3 = Chart3,
        Chart4 = Chart4,
        Chart5 = Chart5,
        Sidebar = Sidebar,
        SidebarForeground = SidebarForeground,
        SidebarPrimary = SidebarPrimary,
        SidebarPrimaryForeground = SidebarPrimaryForeground,
        SidebarAccent = SidebarAccent,
        SidebarAccentForeground = SidebarAccentForeground,
        SidebarBorder = SidebarBorder,
        SidebarRing = SidebarRing,
        BackgroundBlack = BackgroundBlack,
        BackgroundDark = BackgroundDark,
        CardDark = CardDark,
        BorderDark = BorderDark,
        ScrollbarTrack = ScrollbarTrack,
        ScrollbarThumb = ScrollbarThumb,
        ScrollbarThumbHover = ScrollbarThumbHover,
        ChatDark = ChatDark,
        ChatSidebar = ChatSidebar,
        ChatBorder = ChatBorder,
        SidebarText = SidebarText,
        SidebarHeader = SidebarHeader,
        FontSans = FontSans,
        FontSerif = FontSerif,
        FontMono = FontMono,
        TrackingNormal = TrackingNormal,
        Spacing = Spacing,
        ShadowX = ShadowX,
        ShadowY = ShadowY,
        ShadowBlur = ShadowBlur,
        ShadowSpread = ShadowSpread,
        ShadowOpacity = ShadowOpacity,
        ShadowColor = ShadowColor,
        Shadow2xs = Shadow2xs,
        ShadowXs = ShadowXs,
        ShadowSm = ShadowSm,
        Shadow = Shadow,
        ShadowMd = ShadowMd,
        ShadowLg = ShadowLg,
        ShadowXl = ShadowXl,
        Shadow2xl = Shadow2xl
    };

    /// <summary>
    /// Converts the theme state to CSS variable name/value pairs.
    /// </summary>
    public IReadOnlyDictionary<string, string> ToCssVarMap()
    {
        return new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["--radius"] = Radius,
            ["--background"] = Background,
            ["--foreground"] = Foreground,
            ["--card"] = Card,
            ["--card-foreground"] = CardForeground,
            ["--popover"] = Popover,
            ["--popover-foreground"] = PopoverForeground,
            ["--primary"] = Primary,
            ["--primary-foreground"] = PrimaryForeground,
            ["--secondary"] = Secondary,
            ["--secondary-foreground"] = SecondaryForeground,
            ["--muted"] = Muted,
            ["--muted-foreground"] = MutedForeground,
            ["--accent"] = Accent,
            ["--accent-foreground"] = AccentForeground,
            ["--destructive"] = Destructive,
            ["--destructive-foreground"] = DestructiveForeground,
            ["--border"] = Border,
            ["--input"] = Input,
            ["--ring"] = Ring,
            ["--chart-1"] = Chart1,
            ["--chart-2"] = Chart2,
            ["--chart-3"] = Chart3,
            ["--chart-4"] = Chart4,
            ["--chart-5"] = Chart5,
            ["--sidebar"] = Sidebar,
            ["--sidebar-foreground"] = SidebarForeground,
            ["--sidebar-primary"] = SidebarPrimary,
            ["--sidebar-primary-foreground"] = SidebarPrimaryForeground,
            ["--sidebar-accent"] = SidebarAccent,
            ["--sidebar-accent-foreground"] = SidebarAccentForeground,
            ["--sidebar-border"] = SidebarBorder,
            ["--sidebar-ring"] = SidebarRing,
            ["--background-black"] = BackgroundBlack,
            ["--background-dark"] = BackgroundDark,
            ["--card-dark"] = CardDark,
            ["--border-dark"] = BorderDark,
            ["--scrollbar-track"] = ScrollbarTrack,
            ["--scrollbar-thumb"] = ScrollbarThumb,
            ["--scrollbar-thumb-hover"] = ScrollbarThumbHover,
            ["--chat-dark"] = ChatDark,
            ["--chat-sidebar"] = ChatSidebar,
            ["--chat-border"] = ChatBorder,
            ["--sidebar-text"] = SidebarText,
            ["--sidebar-header"] = SidebarHeader,
            ["--font-sans"] = FontSans,
            ["--font-serif"] = FontSerif,
            ["--font-mono"] = FontMono,
            ["--tracking-normal"] = TrackingNormal,
            ["--spacing"] = Spacing,
            ["--shadow-x"] = ShadowX,
            ["--shadow-y"] = ShadowY,
            ["--shadow-blur"] = ShadowBlur,
            ["--shadow-spread"] = ShadowSpread,
            ["--shadow-opacity"] = ShadowOpacity,
            ["--shadow-color"] = ShadowColor,
            ["--shadow-2xs"] = Shadow2xs,
            ["--shadow-xs"] = ShadowXs,
            ["--shadow-sm"] = ShadowSm,
            ["--shadow"] = Shadow,
            ["--shadow-md"] = ShadowMd,
            ["--shadow-lg"] = ShadowLg,
            ["--shadow-xl"] = ShadowXl,
            ["--shadow-2xl"] = Shadow2xl
        };
    }
    /// <summary>
    /// Returns all managed CSS variable names supported by this theme model.
    /// </summary>
    public static string[] GetManagedCssVarNames() =>
        [.. new ThemeState().ToCssVarMap().Keys];

    /// <summary>
    /// Creates a theme state from CSS variable values, falling back to defaults when values are missing.
    /// </summary>
    /// <param name="values">CSS variable values keyed by variable name.</param>
    public static ThemeState FromCssVarMap(IReadOnlyDictionary<string, string> values)
    {
        var state = new ThemeState();
        state.ApplyCssVarMap(values);
        return state;
    }

    /// <summary>
    /// Applies CSS variable values to the current state.
    /// </summary>
    /// <param name="values">CSS variable values keyed by variable name.</param>
    public void ApplyCssVarMap(IReadOnlyDictionary<string, string> values)
    {
        foreach (var pair in values)
        {
            if (string.IsNullOrWhiteSpace(pair.Value))
            {
                continue;
            }

            switch (pair.Key)
            {
                case "--radius": Radius = pair.Value; break;
                case "--background": Background = pair.Value; break;
                case "--foreground": Foreground = pair.Value; break;
                case "--card": Card = pair.Value; break;
                case "--card-foreground": CardForeground = pair.Value; break;
                case "--popover": Popover = pair.Value; break;
                case "--popover-foreground": PopoverForeground = pair.Value; break;
                case "--primary": Primary = pair.Value; break;
                case "--primary-foreground": PrimaryForeground = pair.Value; break;
                case "--secondary": Secondary = pair.Value; break;
                case "--secondary-foreground": SecondaryForeground = pair.Value; break;
                case "--muted": Muted = pair.Value; break;
                case "--muted-foreground": MutedForeground = pair.Value; break;
                case "--accent": Accent = pair.Value; break;
                case "--accent-foreground": AccentForeground = pair.Value; break;
                case "--destructive": Destructive = pair.Value; break;
                case "--destructive-foreground": DestructiveForeground = pair.Value; break;
                case "--border": Border = pair.Value; break;
                case "--input": Input = pair.Value; break;
                case "--ring": Ring = pair.Value; break;
                case "--chart-1": Chart1 = pair.Value; break;
                case "--chart-2": Chart2 = pair.Value; break;
                case "--chart-3": Chart3 = pair.Value; break;
                case "--chart-4": Chart4 = pair.Value; break;
                case "--chart-5": Chart5 = pair.Value; break;
                case "--sidebar": Sidebar = pair.Value; break;
                case "--sidebar-foreground": SidebarForeground = pair.Value; break;
                case "--sidebar-primary": SidebarPrimary = pair.Value; break;
                case "--sidebar-primary-foreground": SidebarPrimaryForeground = pair.Value; break;
                case "--sidebar-accent": SidebarAccent = pair.Value; break;
                case "--sidebar-accent-foreground": SidebarAccentForeground = pair.Value; break;
                case "--sidebar-border": SidebarBorder = pair.Value; break;
                case "--sidebar-ring": SidebarRing = pair.Value; break;
                case "--background-black": BackgroundBlack = pair.Value; break;
                case "--background-dark": BackgroundDark = pair.Value; break;
                case "--card-dark": CardDark = pair.Value; break;
                case "--border-dark": BorderDark = pair.Value; break;
                case "--scrollbar-track": ScrollbarTrack = pair.Value; break;
                case "--scrollbar-thumb": ScrollbarThumb = pair.Value; break;
                case "--scrollbar-thumb-hover": ScrollbarThumbHover = pair.Value; break;
                case "--chat-dark": ChatDark = pair.Value; break;
                case "--chat-sidebar": ChatSidebar = pair.Value; break;
                case "--chat-border": ChatBorder = pair.Value; break;
                case "--sidebar-text": SidebarText = pair.Value; break;
                case "--sidebar-header": SidebarHeader = pair.Value; break;
                case "--font-sans": FontSans = pair.Value; break;
                case "--font-serif": FontSerif = pair.Value; break;
                case "--font-mono": FontMono = pair.Value; break;
                case "--tracking-normal": TrackingNormal = pair.Value; break;
                case "--spacing": Spacing = pair.Value; break;
                case "--shadow-x": ShadowX = pair.Value; break;
                case "--shadow-y": ShadowY = pair.Value; break;
                case "--shadow-blur": ShadowBlur = pair.Value; break;
                case "--shadow-spread": ShadowSpread = pair.Value; break;
                case "--shadow-opacity": ShadowOpacity = pair.Value; break;
                case "--shadow-color": ShadowColor = pair.Value; break;
                case "--shadow-2xs": Shadow2xs = pair.Value; break;
                case "--shadow-xs": ShadowXs = pair.Value; break;
                case "--shadow-sm": ShadowSm = pair.Value; break;
                case "--shadow": Shadow = pair.Value; break;
                case "--shadow-md": ShadowMd = pair.Value; break;
                case "--shadow-lg": ShadowLg = pair.Value; break;
                case "--shadow-xl": ShadowXl = pair.Value; break;
                case "--shadow-2xl": Shadow2xl = pair.Value; break;
            }
        }
    }
}

