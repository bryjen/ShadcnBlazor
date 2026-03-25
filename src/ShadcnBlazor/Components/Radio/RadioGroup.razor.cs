using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using ShadcnBlazor.Components.Shared;
using ShadcnBlazor.Components.Shared.Models.Enums;

namespace ShadcnBlazor.Components.Radio;

/// <summary>
/// Container for Radio or RadioCard options, managing single-selection state.
/// </summary>
public partial class RadioGroup : ShadcnComponentBase, IDisposable
{
    /// <summary>
    /// The Radio or RadioCard options.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The selected value (Value of the selected Radio/RadioCard).
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Callback invoked when the selected value changes.
    /// </summary>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// Expression identifying the bound field (required for EditForm validation).
    /// </summary>
    [Parameter]
    public Expression<Func<string?>>? ValueExpression { get; set; }

    /// <summary>
    /// Size applied to all child radios.
    /// </summary>
    [Parameter]
    public Size Size { get; set; } = Size.Md;

    /// <summary>
    /// Whether all options in the group are disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Whether to lay out options vertically (true) or horizontally (false).
    /// </summary>
    [Parameter]
    public bool Vertical { get; set; } = true;

    [CascadingParameter]
    private EditContext? EditContext { get; set; }

    private EditContext? _subscribedEditContext;

    internal bool IsInvalid => EditContext is not null &&
                               ValueExpression is not null &&
                               EditContext.GetValidationMessages(FieldIdentifier.Create(ValueExpression)).Any();

    protected override void OnParametersSet()
    {
        if (EditContext is not null && ValueExpression is null)
            throw new InvalidOperationException($"{GetType()} requires a value for the 'ValueExpression' parameter when used inside an EditForm.");

        if (!ReferenceEquals(_subscribedEditContext, EditContext))
        {
            if (_subscribedEditContext is not null)
                _subscribedEditContext.OnValidationStateChanged -= HandleValidationStateChanged;

            _subscribedEditContext = EditContext;

            if (_subscribedEditContext is not null)
                _subscribedEditContext.OnValidationStateChanged += HandleValidationStateChanged;
        }
    }

    internal async Task SetValueAsync(string? value)
    {
        if (Disabled)
        {
            return;
        }

        if (string.Equals(Value, value, StringComparison.Ordinal))
        {
            return;
        }

        Value = value;
        await ValueChanged.InvokeAsync(Value);
        if (EditContext is not null && ValueExpression is not null)
            EditContext.NotifyFieldChanged(FieldIdentifier.Create(ValueExpression));
        StateHasChanged();
    }

    private string GetClass()
    {
        var direction = Vertical ? "flex flex-col gap-2" : "flex items-center gap-4";
        return MergeCss(direction, Class);
    }

    private void HandleValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
    {
        _ = InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        if (_subscribedEditContext is not null)
            _subscribedEditContext.OnValidationStateChanged -= HandleValidationStateChanged;
    }
}

