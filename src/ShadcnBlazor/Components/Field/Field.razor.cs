using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using ShadcnBlazor.Components.Shared;
using ShadcnBlazor.Components.Shared.Models.Enums;

namespace ShadcnBlazor.Components.Field;

/// <summary>
/// Context cascaded by Field to all child form controls.
/// </summary>
public record FieldContext(Expression<Func<object?>>? For, string? FieldId);

/// <summary>
/// Field wrapper that manages layout orientation and invalid state styling.
/// </summary>
public partial class Field : ShadcnComponentBase, IDisposable
{
    /// <summary>The content of the field.</summary>
    [Parameter]
    [Category(ComponentCategory.Content)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>The field orientation.</summary>
    [Parameter]
    [Category(ComponentCategory.Appearance)]
    public FieldOrientation Orientation { get; set; } = FieldOrientation.Vertical;

    /// <summary>
    /// The field expression used to bind to EditContext validation.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Validation)]
    public Expression<Func<object?>>? For { get; set; }

    /// <summary>
    /// Optional id of the associated form control. Used by FieldLabel and FieldError.
    /// </summary>
    [Parameter]
    [Category(ComponentCategory.Common)]
    public string? FieldId { get; set; }

    /// <summary>Current form edit context (if inside an EditForm).</summary>
    [CascadingParameter]
    public EditContext? EditContext { get; set; }

    private EditContext? _subscribedEditContext;
    private FieldContext? _fieldContext;

    internal bool IsInvalid => GetErrors().Any();

    internal string? ErrorMessageId => string.IsNullOrWhiteSpace(FieldId) ? null : $"{FieldId}-error";

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        _fieldContext = new(For, FieldId);

        if (!ReferenceEquals(_subscribedEditContext, EditContext))
        {
            if (_subscribedEditContext is not null)
                _subscribedEditContext.OnValidationStateChanged -= HandleValidationStateChanged;

            _subscribedEditContext = EditContext;

            if (_subscribedEditContext is not null)
                _subscribedEditContext.OnValidationStateChanged += HandleValidationStateChanged;
        }
    }

    internal IEnumerable<string> GetErrors()
    {
        if (EditContext is null || For is null)
            return Array.Empty<string>();

        var fieldIdentifier = FieldIdentifier.Create(For);
        return EditContext.GetValidationMessages(fieldIdentifier);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_subscribedEditContext is not null)
            _subscribedEditContext.OnValidationStateChanged -= HandleValidationStateChanged;
    }

    private void HandleValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
    {
        _ = InvokeAsync(StateHasChanged);
    }

    private string GetClass()
    {
        var baseClasses = "group/field flex w-full gap-3 data-[invalid=true]:text-destructive";
        var orientationClasses = Orientation switch
        {
            FieldOrientation.Vertical =>
                "flex-col [&>*]:w-full [&>.sr-only]:w-auto",
            FieldOrientation.Horizontal =>
                "flex-row items-center " +
                "[&>[data-slot=field-label]]:flex-auto " +
                "has-[>[data-slot=field-content]]:items-start " +
                "has-[>[data-slot=field-content]]:[&>[role=checkbox],[role=radio]]:mt-px",
            FieldOrientation.Responsive =>
                "flex-col @md/field-group:flex-row @md/field-group:items-center " +
                "[&>*]:w-full @md/field-group:[&>*]:w-auto [&>.sr-only]:w-auto " +
                "@md/field-group:[&>[data-slot=field-label]]:flex-auto " +
                "@md/field-group:has-[>[data-slot=field-content]]:items-start " +
                "@md/field-group:has-[>[data-slot=field-content]]:[&>[role=checkbox],[role=radio]]:mt-px",
            _ => "flex-col [&>*]:w-full [&>.sr-only]:w-auto",
        };

        return MergeCss(baseClasses, orientationClasses, Class);
    }
}
