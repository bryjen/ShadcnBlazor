namespace SampleWasmProject.Components.Core.Dialog.Models;

/// <summary>
/// Parameters passed to dialog content components.
/// </summary>
public class DialogParameters : Dictionary<string, object?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DialogParameters"/> class.
    /// </summary>
    public DialogParameters()
    {
    }

    /// <summary>
    /// Adds a parameter with the specified name and value.
    /// </summary>
    /// <param name="parameterName">The parameter name.</param>
    /// <param name="value">The parameter value.</param>
    public void Add<T>(string parameterName, T value)
    {
        this[parameterName] = value;
    }
}
