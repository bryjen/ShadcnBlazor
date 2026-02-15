using Microsoft.AspNetCore.Components;
using TailwindMerge;

namespace WasmStandalone.Shared;

/// <summary>
/// Base class for all ShadcnBlazor components, providing Tailwind class merging and common parameters.
/// </summary>
public abstract class ShadcnComponentBase : ComponentBase
{
    /// <summary>
    /// Injected TailwindMerge service for merging Tailwind CSS classes.
    /// </summary>
    [Inject]
    public required TwMerge TwMerge { get; set; }

    /// <summary>
    /// Additional CSS classes to apply to the component.
    /// </summary>
    [Parameter]
    public string Class { get; set; } = string.Empty;

    /// <summary>
    /// Additional HTML attributes to apply to the root element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Merges component classes with user-provided classes using TailwindMerge.
    /// </summary>
    /// <param name="classes">The CSS classes to merge.</param>
    /// <returns>The merged class string.</returns>
    protected string MergeCss(params string[] classes)
    {
        var joined = string.Join(" ", classes) + " " + Class;
        return TwMerge.Merge(joined) ?? joined;
    }
}
