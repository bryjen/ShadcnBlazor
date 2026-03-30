using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Components.DataTable;

/// <summary>Defines a column for <see cref="DataTable{T}"/>.</summary>
public partial class DataTableColumn<T> : ShadcnComponentBase, IDisposable
{
    /// <summary>Owning table context.</summary>
    [CascadingParameter] public DataTableContext<T> Context { get; set; } = default!;

    /// <summary>Header text for the column.</summary>
    [Parameter] public string Title { get; set; } = "";
    /// <summary>Value selector used for sorting and default rendering.</summary>
    [Parameter] public Func<T, object?>? Field { get; set; }
    /// <summary>Custom cell rendering template.</summary>
    [Parameter] public RenderFragment<T>? CellTemplate { get; set; }
    /// <summary>Whether this column is sortable.</summary>
    [Parameter] public bool Sortable { get; set; }
    /// <summary>Optional sort key used in server mode.</summary>
    [Parameter] public string? SortKey { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Context.Register(this);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Context.Remove(this);
    }
}
