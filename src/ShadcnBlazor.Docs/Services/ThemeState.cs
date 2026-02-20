namespace ShadcnBlazor.Docs.Services;

/// <summary>
/// Represents the current theme values for the docs app.
/// </summary>
public sealed class ThemeState
{
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
    /// <summary>Destructive foreground color (optional).</summary>
    public string? DestructiveForeground { get; set; }
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

    /// <summary>
    /// Creates a deep copy of the current state.
    /// </summary>
    public ThemeState Clone() => new()
    {
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
        SidebarRing = SidebarRing
    };

    /// <summary>
    /// Converts the theme state to CSS variable name/value pairs.
    /// </summary>
    public IReadOnlyDictionary<string, string> ToCssVarMap()
    {
        var map = new Dictionary<string, string>(StringComparer.Ordinal)
        {
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
            ["--sidebar-ring"] = SidebarRing
        };

        if (!string.IsNullOrWhiteSpace(DestructiveForeground))
        {
            map["--destructive-foreground"] = DestructiveForeground;
        }

        return map;
    }
}
