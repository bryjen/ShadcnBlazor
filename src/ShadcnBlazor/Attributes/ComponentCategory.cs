namespace ShadcnBlazor;

/// <summary>Predefined property categories for API documentation grouping.</summary>
public enum ComponentCategory
{
    /// <summary>Data properties (values, state).</summary>
    Data = 0,

    /// <summary>Validation-related properties.</summary>
    Validation = 10,

    /// <summary>Content/child content properties (RenderFragment).</summary>
    Content = 50,

    /// <summary>Behavior properties (events, callbacks, behavior flags).</summary>
    Behavior = 100,

    /// <summary>Item collections and data arrays.</summary>
    Items = 150,

    /// <summary>Appearance/styling properties (variants, sizes, colors).</summary>
    Appearance = 300,

    /// <summary>State-related properties.</summary>
    States = 350,

    /// <summary>Common/miscellaneous properties (fallback category, highest sort order).</summary>
    Common = int.MaxValue,
}
