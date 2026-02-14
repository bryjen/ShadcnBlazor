using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Shared;
using ShadcnBlazor.Shared.Attributes;

namespace ShadcnBlazor.Components.Accordion;

/// <summary>
/// A vertically stacked set of interactive headings that each reveal a section of content.
/// Use with <see cref="AccordionItem"/>, <see cref="AccordionTrigger"/>, and <see cref="AccordionContent"/>.
/// </summary>
[ComponentMetadata(Name = nameof(Accordion), Description = "A vertically stacked set of interactive headings that each reveal a section of content..", Dependencies = [])]
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
    /// </summary>
    [Parameter]
    [Category("Behavior")]
    public bool Collapsible { get; set; }

    /// <summary>
    /// The value of the item to open by default when uncontrolled.
    /// Must match the <c>Value</c> of an <see cref="AccordionItem"/>.
    /// </summary>
    [Parameter]
    [Category("Behavior")]
    public string? DefaultValue { get; set; }

    /// <summary>
    /// The value of the currently open item when controlled.
    /// Use with <see cref="ValueChanged"/> for two-way binding.
    /// </summary>
    [Parameter]
    [Category("Behavior")]
    public string? Value { get; set; }

    /// <summary>
    /// Callback fired when the open item changes. Use with <see cref="Value"/> for two-way binding.
    /// </summary>
    [Parameter]
    [Category("Behavior")]
    public EventCallback<string?> ValueChanged { get; set; }

    private string? _localValue;
    private AccordionContext _context = null!;

    protected override void OnInitialized()
    {
        _localValue = DefaultValue;
        _context = new AccordionContext(this);
    }

    protected override void OnParametersSet()
    {
        if (IsControlled)
            _localValue = Value;
    }

    private bool IsControlled => ValueChanged.HasDelegate;

    private string? CurrentValue => IsControlled ? Value : _localValue;

    internal bool IsItemOpen(string value) => string.Equals(CurrentValue, value, StringComparison.Ordinal);

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
    }

    private async Task SetValueAsync(string? value)
    {
        if (IsControlled)
        {
            await ValueChanged.InvokeAsync(value);
        }
        else
        {
            _localValue = value;
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
