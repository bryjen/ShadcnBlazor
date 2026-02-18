using Microsoft.AspNetCore.Components;
using ShadcnBlazor.Components.Sheet.Models;

namespace ShadcnBlazor.Components.Sheet.Services;

/// <summary>
/// Implementation of <see cref="ISheetService"/> for imperative sheet display.
/// </summary>
public class SheetService : ISheetService
{
    /// <summary>
    /// List of currently open sheets.
    /// </summary>
    internal List<SheetInstance> SheetInstances { get; } = [];

    /// <summary>
    /// Event raised when the sheet list changes (open/close).
    /// </summary>
    public event Action? OnSheetsChanged;

    /// <inheritdoc />
    public ISheetReference Show<T>(SheetParameters? parameters = null)
        where T : ComponentBase
    {
        var id = Guid.NewGuid();
        var sheetReference = new SheetReference(id, this);

        var sheetInstance = new SheetInstance
        {
            Id = id,
            ComponentType = typeof(T),
            Parameters = parameters ?? new SheetParameters(),
            TaskCompletionSource = new TaskCompletionSource<SheetResult>(),
            Reference = sheetReference
        };

        SheetInstances.Add(sheetInstance);
        OnSheetsChanged?.Invoke();

        return sheetReference;
    }

    /// <summary>
    /// Closes the sheet with the specified reference and result.
    /// </summary>
    internal async Task CloseAsync(SheetReference reference, SheetResult result)
    {
        var instance = SheetInstances.FirstOrDefault(x => x.Id == reference.Id);
        if (instance == null) return;

        var handler = instance.CloseHandler;
        if (handler != null)
        {
            try
            {
                await handler(result);
            }
            catch
            {
                if (SheetInstances.Contains(instance))
                    CompleteClose(instance, result);
            }
            return;
        }

        CompleteClose(instance, result);
    }

    /// <summary>
    /// Completes the close by removing the instance and setting the result.
    /// </summary>
    internal void CompleteClose(SheetReference reference, SheetResult result)
    {
        var instance = SheetInstances.FirstOrDefault(x => x.Id == reference.Id);
        if (instance == null) return;
        CompleteClose(instance, result);
    }

    private void CompleteClose(SheetInstance instance, SheetResult result)
    {
        if (!SheetInstances.Remove(instance)) return;
        instance.TaskCompletionSource.TrySetResult(result);
        OnSheetsChanged?.Invoke();
    }
}
