using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using ShadcnBlazor.Components.Shared;
using ShadcnBlazor.Components.Shared.Models.Enums;
using TailwindMerge;

#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.

namespace ShadcnBlazor.Components.Checkbox;

/// <summary>
/// Checkbox input for boolean or multi-select form values.
/// </summary>
public partial class Checkbox : ShadcnComponentBase, IDisposable
{
    /// <summary>
    /// The unique identifier for this checkbox, used for associating with a label via <c>htmlFor</c>.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Common)]
    public string? Id { get; set; }

    /// <summary>
    /// The current value of the checkbox.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Data)]
    public bool Value { get; set; }

    /// <summary>
    /// Callback invoked when the value changes.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public EventCallback<bool> ValueChanged { get; set; }

    /// <summary>
    /// Expression identifying the bound field (required for EditForm validation).
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Validation)]
    public Expression<Func<bool>>? ValueExpression { get; set; }

    /// <summary>
    /// The size of the checkbox.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Appearance)]
    public Size Size { get; set; } = Size.Md;

    /// <summary>
    /// Whether the checkbox is disabled.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Behavior)]
    public bool Disabled { get; set; }

    /// <summary>
    /// Whether the checkbox is in an invalid state. When true, sets <c>aria-invalid="true"</c> and applies invalid styling.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Validation)]
    public bool Invalid { get; set; }

    [CascadingParameter]
    private EditContext? EditContext { get; set; }

    private EditContext? _subscribedEditContext;

    private FieldIdentifier FieldIdentifier => ValueExpression is null ? default : FieldIdentifier.Create(ValueExpression);

    private bool HasValidationMessages => EditContext is not null &&
                                         ValueExpression is not null &&
                                         EditContext.GetValidationMessages(FieldIdentifier).Any();

    internal bool IsInvalid => Invalid || HasValidationMessages;

    /// <inheritdoc />
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

    private string GetClass()
    {
        var baseClasses = "peer flex items-center justify-center p-0 border-input bg-input/30 data-[state=checked]:bg-primary data-[state=checked]:text-primary-foreground data-[state=checked]:border-primary focus-visible:border-ring focus-visible:ring-ring/50 aria-invalid:ring-destructive/40 aria-invalid:border-destructive shrink-0 rounded-[4px] border shadow-xs transition-all duration-200 outline-none focus-visible:ring-[3px] disabled:cursor-not-allowed disabled:opacity-50";
        var sizeClasses = Size switch
        {
            Size.Sm => "size-3",
            Size.Md => "size-4",
            Size.Lg => "size-5",
        };

        return MergeCss(baseClasses, sizeClasses, Class ?? "");
    }

    private string GetCheckmarkClass() => Size switch
    {
        Size.Sm => "size-2",
        Size.Md => "size-2.5",
        Size.Lg => "size-4",
        _ => "size-2.5",
    };

    private string GetIndicatorClass()
    {
        if (Value)
        {
            return "grid place-content-center size-full text-current animate-in zoom-in-50 fade-in-0 duration-200";
        }
        else
        {
            return "grid place-content-center size-full text-current animate-out zoom-out-50 fade-out-0 duration-150 pointer-events-none opacity-0";
        }
    }

    private string GetAriaChecked()
    {
        return Value ? "true" : "false";
    }

    private async Task HandleClick()
    {
        if (!Disabled)
        {
            Value = !Value;
            await ValueChanged.InvokeAsync(Value);
            if (EditContext is not null && ValueExpression is not null)
                EditContext.NotifyFieldChanged(FieldIdentifier);
        }
    }

    private void HandleValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
    {
        _ = InvokeAsync(StateHasChanged);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_subscribedEditContext is not null)
            _subscribedEditContext.OnValidationStateChanged -= HandleValidationStateChanged;
    }
}

