namespace ShadcnBlazor;

/// <summary>
/// Attribute to define the category of a component property.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class CategoryAttribute : Attribute
{
    private static readonly Dictionary<ComponentCategory, string> CategoryNames = new()
    {
        { ComponentCategory.Data, "Data" },
        { ComponentCategory.Validation, "Validation" },
        { ComponentCategory.Content, "Content" },
        { ComponentCategory.Behavior, "Behavior" },
        { ComponentCategory.Items, "Items" },
        { ComponentCategory.Appearance, "Appearance" },
        { ComponentCategory.States, "States" },
        { ComponentCategory.Common, "Common" },
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="CategoryAttribute"/> class.
    /// </summary>
    /// <param name="category">The component category.</param>
    public CategoryAttribute(ComponentCategory category) { Category = category; }

    /// <summary>
    /// Gets the component category.
    /// </summary>
    public ComponentCategory Category { get; }

    /// <summary>
    /// Gets the display name of the category.
    /// </summary>
    public string Name  => CategoryNames.TryGetValue(Category, out var name) ? name : "Common";

    /// <summary>
    /// Gets the sort order of the category.
    /// </summary>
    public int    Order => (int)Category;
}
