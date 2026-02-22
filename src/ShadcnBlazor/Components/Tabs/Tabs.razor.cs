using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Components.Tabs;

/// <summary>
/// A set of layered sections of content—known as tab panels—that are displayed one at a time.
/// Use with <see cref="TabsList"/>, <see cref="TabsTrigger"/>, and <see cref="TabsContent"/>.
/// </summary>
public partial class Tabs : ShadcnComponentBase
{
    /// <summary>
    /// The content of the tabs, typically a <see cref="TabsList"/> and one or more <see cref="TabsContent"/> components.
    /// </summary>
    [Parameter]
    [Category("Content")]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The value of the tab to select by default when uncontrolled.
    /// </summary>
    [Parameter]
    [Category("Behavior")]
    public string? DefaultValue { get; set; }

    /// <summary>
    /// The currently selected tab value when controlled. Use with <see cref="ValueChanged"/> for two-way binding.
    /// </summary>
    [Parameter]
    [Category("Behavior")]
    public string? Value { get; set; }

    /// <summary>
    /// Callback fired when the selected tab changes.
    /// </summary>
    [Parameter]
    [Category("Behavior")]
    public EventCallback<string> ValueChanged { get; set; }

    /// <summary>
    /// The orientation of the tabs. Use "horizontal" (default) or "vertical".
    /// </summary>
    [Parameter]
    [Category("Behavior")]
    public string Orientation { get; set; } = "horizontal";

    private string? _localValue;
    private TabsContext _context = null!;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        _localValue = DefaultValue;
        _context = new TabsContext
        {
            ActiveValue = IsControlled ? Value : _localValue,
            Orientation = Orientation,
            OnTabSelected = SelectTabAsync
        };
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        _context.Orientation = Orientation;
        _context.ActiveValue = IsControlled ? Value : _localValue;
    }

    private bool IsControlled => ValueChanged.HasDelegate;

    private async Task SelectTabAsync(string value)
    {
        if (IsControlled)
        {
            await ValueChanged.InvokeAsync(value);
        }
        else
        {
            _localValue = value;
            _context.ActiveValue = value;
        }

        await InvokeAsync(StateHasChanged);
    }

    private string GetClass()
    {
        var baseClasses = Orientation == "vertical"
            ? "flex flex-row"
            : "flex flex-col";
        return MergeCss(baseClasses, Class);
    }
}
