namespace ShadcnBlazor.Components.DataTable;

/// <summary>Tracks column registrations for a data table.</summary>
public sealed class DataTableContext<T>
{
    private readonly List<DataTableColumn<T>> _columns = [];
    /// <summary>Registered columns in declaration order.</summary>
    public IReadOnlyList<DataTableColumn<T>> Columns => _columns;
    /// <summary>Raised when the column collection changes.</summary>
    public event Action? Changed;
    /// <summary>Adds a column to the context.</summary>
    public void Register(DataTableColumn<T> col) { _columns.Add(col); Changed?.Invoke(); }
    /// <summary>Removes a column from the context.</summary>
    public void Remove(DataTableColumn<T> col)   { if (_columns.Remove(col)) Changed?.Invoke(); }
}
