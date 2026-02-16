using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Components.Accordion;

/// <summary>
/// A vertically stacked set of interactive headings that each reveal a section of content.
/// Use with <see cref="AccordionItem"/>, <see cref="AccordionTrigger"/>, and <see cref="AccordionContent"/>.
/// </summary>
public partial class Accordion : ShadcnComponentBase
{
    /// <summary>
    /// The content of the accordion, typically one or more <see cref="AccordionItem"/> components.
    /// </summary>
    [Parameter]
    [Category("Content")]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Whether only one item can be open at a time (<see cref="AccordionType.Single"/>) or multiple items (<see cref="AccordionType.Multiple"/>).
    /// </summary>
    [Parameter]
    [Category("Behavior")]
    public AccordionType Type { get; set; } = AccordionType.Single;

    /// <summary>
    /// When <see cref="AccordionType.Single"/>, allows the open item to be collapsed so that no item is open.
    /// Defaults to true so clicking an open item closes it. Set to false to require at least one item to remain open.
    /// </summary>
    [Parameter]
    [Category("Behavior")]
    public bool Collapsible { get; set; } = true;

    /// <summary>
    /// The value of the item to open by default when uncontrolled.
    /// Must match the <c>Value</c> of an <see cref="AccordionItem"/>.
    /// Used when <see cref="Type"/> is <see cref="AccordionType.Single"/>.
    /// </summary>
    [Parameter]
    [Category("Behavior")]
    public string? DefaultValue { get; set; }

    /// <summary>
    /// The values of items to open by default when uncontrolled.
    /// Must match the <c>Value</c> of <see cref="AccordionItem"/> components.
    /// Used when <see cref="Type"/> is <see cref="AccordionType.Multiple"/>.
    /// </summary>
    [Parameter]
    [Category("Behavior")]
    public IEnumerable<string>? DefaultValues { get; set; }

    /// <summary>
    /// The value of the currently open item when controlled.
    /// Use with <see cref="ValueChanged"/> for two-way binding.
    /// Used when <see cref="Type"/> is <see cref="AccordionType.Single"/>.
    /// </summary>
    [Parameter]
    [Category("Behavior")]
    public string? Value { get; set; }

    /// <summary>
    /// The values of the currently open items when controlled.
    /// Use with <see cref="ValuesChanged"/> for two-way binding.
    /// Used when <see cref="Type"/> is <see cref="AccordionType.Multiple"/>.
    /// </summary>
    [Parameter]
    [Category("Behavior")]
    public IEnumerable<string>? Values { get; set; }

    /// <summary>
    /// Callback fired when the open item changes. Use with <see cref="Value"/> for two-way binding.
    /// Used when <see cref="Type"/> is <see cref="AccordionType.Single"/>.
    /// </summary>
    [Parameter]
    [Category("Behavior")]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// Callback fired when the open items change. Use with <see cref="Values"/> for two-way binding.
    /// Used when <see cref="Type"/> is <see cref="AccordionType.Multiple"/>.
    /// </summary>
    [Parameter]
    [Category("Behavior")]
    public EventCallback<IEnumerable<string>> ValuesChanged { get; set; }

    private string? _localValue;
    private HashSet<string> _localValues = [];
    private AccordionContext _context = null!;

    protected override void OnInitialized()
    {
        _localValue = DefaultValue;
        _localValues = DefaultValues?.ToHashSet(StringComparer.Ordinal) ?? [];
        _context = new AccordionContext(this);
    }

    protected override void OnParametersSet()
    {
        if (Type == AccordionType.Single && IsControlledSingle)
            _localValue = Value;
        if (Type == AccordionType.Multiple && IsControlledMultiple)
            _localValues = Values?.ToHashSet(StringComparer.Ordinal) ?? [];
    }

    private bool IsControlledSingle => ValueChanged.HasDelegate;
    private bool IsControlledMultiple => ValuesChanged.HasDelegate;

    private string? CurrentValue => IsControlledSingle ? Value : _localValue;

    private HashSet<string> CurrentValues =>
        Type == AccordionType.Multiple && IsControlledMultiple
            ? (Values?.ToHashSet(StringComparer.Ordinal) ?? [])
            : _localValues;

    internal bool IsItemOpen(string value) =>
        Type == AccordionType.Single
            ? string.Equals(CurrentValue, value, StringComparison.Ordinal)
            : CurrentValues.Contains(value);

    internal async Task ToggleItemAsync(string value)
    {
        if (Type == AccordionType.Single)
        {
            if (IsItemOpen(value))
            {
                if (Collapsible)
                    await SetValueAsync(null);

                return;
            }

            await SetValueAsync(value);
            return;
        }

        // Multiple
        var newValues = CurrentValues.ToHashSet(StringComparer.Ordinal);
        if (newValues.Contains(value))
            newValues.Remove(value);
        else
            newValues.Add(value);

        await SetValuesAsync(newValues);
    }

    private async Task SetValueAsync(string? value)
    {
        if (IsControlledSingle)
        {
            await ValueChanged.InvokeAsync(value);
        }
        else
        {
            _localValue = value;
        }

        await InvokeAsync(StateHasChanged);
    }

    private async Task SetValuesAsync(HashSet<string> values)
    {
        if (IsControlledMultiple)
        {
            await ValuesChanged.InvokeAsync(values);
        }
        else
        {
            _localValues = values;
        }

        await InvokeAsync(StateHasChanged);
    }

    private string GetClass()
    {
        var baseClasses = "w-full";
        return MergeCss(baseClasses, Class);
    }

    internal sealed class AccordionContext
    {
        private readonly Accordion _owner;

        public AccordionContext(Accordion owner)
        {
            _owner = owner;
        }

        public bool IsItemOpen(string value) => _owner.IsItemOpen(value);

        public Task ToggleItemAsync(string value) => _owner.ToggleItemAsync(value);
    }

}

/// <summary>
/// Specifies whether the accordion allows one or multiple items to be open at a time.
/// </summary>
public enum AccordionType
{
    /// <summary>Only one item can be open at a time.</summary>
    Single,

    /// <summary>Multiple items can be open at the same time.</summary>
    Multiple
}
