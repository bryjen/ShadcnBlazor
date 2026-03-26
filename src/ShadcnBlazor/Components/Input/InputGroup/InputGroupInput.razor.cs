using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Components.Input.InputGroup;

/// <summary>
/// Input wrapper for use within an input group (strips standalone styling).
/// </summary>
public partial class InputGroupInput : ShadcnComponentBase
{
    /// <summary>
    /// The current value of the input.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Data)]
    public string? Value { get; set; }

    /// <summary>
    /// Callback invoked when the value changes.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public EventCallback<string> ValueChanged { get; set; }

    /// <summary>
    /// The HTML input type (e.g., text, password, email).
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public string? Type { get; set; } = "text";

    /// <summary>
    /// Placeholder text when the value is empty.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Common)]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Whether the input is disabled.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public bool Disabled { get; set; }

    private string GetClass()
    {
        var baseClasses = "flex-1 rounded-none border-0 bg-transparent shadow-none ring-0 focus-visible:ring-0 " +
                          "disabled:bg-transparent aria-invalid:ring-0 dark:bg-transparent";

        return MergeCss(baseClasses, Class ?? string.Empty);
    }
}
