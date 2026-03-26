namespace ShadcnBlazor;

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

    public CategoryAttribute(ComponentCategory category) { Category = category; }
    public ComponentCategory Category { get; }
    public string Name  => CategoryNames.TryGetValue(Category, out var name) ? name : "Common";
    public int    Order => (int)Category;
}
