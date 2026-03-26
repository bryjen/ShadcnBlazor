namespace ShadcnBlazor.Services;

public sealed class VaulOptions
{
    public VaulDrawerOptions? Drawer { get; set; }
    public VaulOverlayOptions? Overlay { get; set; }
    public VaulContentOptions? Content { get; set; }
    public VaulHandleOptions? Handle { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool? A11yHidden { get; set; }
}

public sealed class VaulDrawerOptions
{
    public bool? Dismissible { get; set; }
    public bool? Modal { get; set; }
    public string? Direction { get; set; }
    public double? CloseThreshold { get; set; }
    public int? ScrollLockTimeout { get; set; }
    public bool? HandleOnly { get; set; }
    public bool? Fixed { get; set; }
    public bool? Nested { get; set; }
    public bool? RepositionInputs { get; set; }
    public bool? PreventScrollRestoration { get; set; }
    public bool? DisablePreventScroll { get; set; }
    public bool? AutoFocus { get; set; }
    public bool? ShouldScaleBackground { get; set; }
    public bool? SetBackgroundColorOnScale { get; set; }
    public string? BackgroundColor { get; set; }
    public bool? NoBodyStyles { get; set; }
    public int? FadeFromIndex { get; set; }
    public bool? SnapToSequentialPoint { get; set; }
    public object[]? SnapPoints { get; set; }
    public object? ActiveSnapPoint { get; set; }
    public Dictionary<string, object?>? AdditionalProps { get; set; }
}

public sealed class VaulOverlayOptions
{
    public string? ClassName { get; set; }
    public bool? DisableBaseClass { get; set; }
    public Dictionary<string, object?>? AdditionalProps { get; set; }
}

public sealed class VaulContentOptions
{
    public string? ClassName { get; set; }
    public bool? DisableBaseClass { get; set; }
    public Dictionary<string, object?>? AdditionalProps { get; set; }
}

public sealed class VaulHandleOptions
{
    public bool? PreventCycle { get; set; }
    public Dictionary<string, object?>? AdditionalProps { get; set; }
}
