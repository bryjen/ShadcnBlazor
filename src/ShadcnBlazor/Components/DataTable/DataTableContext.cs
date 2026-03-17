namespace ShadcnBlazor.Components.DataTable;

public sealed class DataTableContext<T>
{
    private readonly List<DataTableColumn<T>> _columns = [];
    public IReadOnlyList<DataTableColumn<T>> Columns => _columns;
    public event Action? Changed;
    public void Register(DataTableColumn<T> col) { _columns.Add(col); Changed?.Invoke(); }
    public void Remove(DataTableColumn<T> col)   { if (_columns.Remove(col)) Changed?.Invoke(); }
}
