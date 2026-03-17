using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Shared;

namespace ShadcnBlazor.Components.DataTable;

public partial class DataTableColumn<T> : ShadcnComponentBase, IDisposable
{
    [CascadingParameter] public DataTableContext<T> Context { get; set; } = default!;

    [Parameter] public string Title { get; set; } = "";
    [Parameter] public Func<T, object?>? Field { get; set; }
    [Parameter] public RenderFragment<T>? CellTemplate { get; set; }
    [Parameter] public bool Sortable { get; set; }
    [Parameter] public string? SortKey { get; set; }

    protected override void OnInitialized()
    {
        Context.Register(this);
    }

    public void Dispose()
    {
        Context.Remove(this);
    }
}
