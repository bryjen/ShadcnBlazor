namespace ShadcnBlazor.Services;

/// <summary>
/// Root options for a Vaul drawer.
/// </summary>
public sealed class DrawerOptions
{
    /// <summary>Options for the drawer itself.</summary>
    public DrawerOptionsInner? Drawer { get; set; }
    /// <summary>Options for the overlay.</summary>
    public DrawerOverlayOptions? Overlay { get; set; }
    /// <summary>Options for the drawer content.</summary>
    public DrawerContentOptions? Content { get; set; }
    /// <summary>Options for the drawer handle.</summary>
    public DrawerHandleOptions? Handle { get; set; }
    /// <summary>Accessible title for the drawer.</summary>
    public string? Title { get; set; }
    /// <summary>Accessible description for the drawer.</summary>
    public string? Description { get; set; }
    /// <summary>Whether the drawer is hidden from screen readers.</summary>
    public bool? A11yHidden { get; set; }
}

/// <summary>Detailed configuration for a Vaul drawer.</summary>
public sealed class DrawerOptionsInner
{
    /// <summary>Whether the drawer can be dismissed by clicking outside or dragging.</summary>
    public bool? Dismissible { get; set; }
    /// <summary>Whether the drawer is modal.</summary>
    public bool? Modal { get; set; }
    /// <summary>The direction the drawer opens from (top, bottom, left, right).</summary>
    public string? Direction { get; set; }
    /// <summary>The threshold for closing the drawer on drag.</summary>
    public double? CloseThreshold { get; set; }
    /// <summary>Timeout for scroll lock removal.</summary>
    public int? ScrollLockTimeout { get; set; }
    /// <summary>Whether interaction is limited to the handle.</summary>
    public bool? HandleOnly { get; set; }
    /// <summary>Whether the drawer has a fixed position.</summary>
    public bool? Fixed { get; set; }
    /// <summary>Whether the drawer is nested.</summary>
    public bool? Nested { get; set; }
    /// <summary>Whether to reposition inputs on focus.</summary>
    public bool? RepositionInputs { get; set; }
    /// <summary>Whether to prevent scroll restoration on close.</summary>
    public bool? PreventScrollRestoration { get; set; }
    /// <summary>Whether to disable scroll prevention.</summary>
    public bool? DisablePreventScroll { get; set; }
    /// <summary>Whether to automatically focus the first element.</summary>
    public bool? AutoFocus { get; set; }
    /// <summary>Whether to scale the background when the drawer is open.</summary>
    public bool? ShouldScaleBackground { get; set; }
    /// <summary>Whether to set background color on scale.</summary>
    public bool? SetBackgroundColorOnScale { get; set; }
    /// <summary>Custom background color for the scale effect.</summary>
    public string? BackgroundColor { get; set; }
    /// <summary>Whether to skip applying body styles.</summary>
    public bool? NoBodyStyles { get; set; }
    /// <summary>Index from which to start fading.</summary>
    public int? FadeFromIndex { get; set; }
    /// <summary>Whether to snap to the sequential point.</summary>
    public bool? SnapToSequentialPoint { get; set; }
    /// <summary>Snap points for the drawer.</summary>
    public object[]? SnapPoints { get; set; }
    /// <summary>The active snap point.</summary>
    public object? ActiveSnapPoint { get; set; }
    /// <summary>Additional properties to pass to the drawer component.</summary>
    public Dictionary<string, object?>? AdditionalProps { get; set; }
}

/// <summary>Options for the Vaul overlay.</summary>
public sealed class DrawerOverlayOptions
{
    /// <summary>Custom CSS class for the overlay.</summary>
    public string? ClassName { get; set; }
    /// <summary>Whether to disable the base overlay class.</summary>
    public bool? DisableBaseClass { get; set; }
    /// <summary>Additional properties for the overlay.</summary>
    public Dictionary<string, object?>? AdditionalProps { get; set; }
}

/// <summary>Options for the Vaul content area.</summary>
public sealed class DrawerContentOptions
{
    /// <summary>Custom CSS class for the content area.</summary>
    public string? ClassName { get; set; }
    /// <summary>Whether to disable the base content class.</summary>
    public bool? DisableBaseClass { get; set; }
    /// <summary>Additional properties for the content area.</summary>
    public Dictionary<string, object?>? AdditionalProps { get; set; }
}

/// <summary>Options for the Vaul drawer handle.</summary>
public sealed class DrawerHandleOptions
{
    /// <summary>Whether to prevent cycling through snap points.</summary>
    public bool? PreventCycle { get; set; }
    /// <summary>Additional properties for the handle.</summary>
    public Dictionary<string, object?>? AdditionalProps { get; set; }
}
