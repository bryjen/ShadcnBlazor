using Microsoft.AspNetCore.Components;

namespace ShadcnBlazor.Components.DataTable;

public partial class DataTableColumn<T> : ComponentBase, IDisposable
{
    [CascadingParameter] public DataTableContext<T> Context { get; set; } = default!;

    [Parameter] public string Title { get; set; } = "";
    [Parameter] public Func<T, object?>? Field { get; set; }
    [Parameter] public RenderFragment<T>? CellTemplate { get; set; }
    [Parameter] public bool Sortable { get; set; }

    protected override void OnInitialized()
    {
        Context.Register(this);
    }

    public void Dispose()
    {
        Context.Remove(this);
    }
}
