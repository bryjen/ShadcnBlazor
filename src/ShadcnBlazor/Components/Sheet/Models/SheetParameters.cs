namespace ShadcnBlazor.Components.Sheet.Models;

/// <summary>
/// Parameters passed to sheet content components.
/// </summary>
public class SheetParameters : Dictionary<string, object?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SheetParameters"/> class.
    /// </summary>
    public SheetParameters()
    {
    }

    /// <summary>
    /// Adds a parameter with the specified name and value.
    /// </summary>
    public void Add<T>(string parameterName, T value)
    {
        this[parameterName] = value;
    }
}
