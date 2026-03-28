using System.Linq;
using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Components.Field;

/// <summary>
/// Validation error display for a field.
/// </summary>
public partial class FieldError : ShadcnComponentBase
{
    [CascadingParameter]
    private Field? ParentField { get; set; }

    internal string? FirstError => ParentField?.GetErrors().FirstOrDefault();

    internal bool HasError => !string.IsNullOrWhiteSpace(FirstError);

    internal string? ErrorMessageId
    {
        get
        {
            if (AdditionalAttributes is not null && AdditionalAttributes.ContainsKey("id"))
                return null;
            return ParentField?.ErrorMessageId;
        }
    }

    private string GetClass()
    {
        return MergeCss("text-sm font-normal text-destructive", Class);
    }
}
